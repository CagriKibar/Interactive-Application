using System.Globalization;
using System.Xml.Linq;
using FinancialERP.Business.Services.Abstract;
using FinancialERP.DataAccess.Repositories.Abstract;
using FinancialERP.Entity.Enums;
using FinancialERP.Entity.Models;
using Microsoft.Extensions.Logging;

namespace FinancialERP.Business.Services.Concrete
{
    /// <summary>
    /// Makroekonomik Veri Entegrasyon Servisi
    ///
    /// TCMB (Türkiye Cumhuriyet Merkez Bankası) API'sinden günlük döviz kurlarını,
    /// TÜİK verilerinden enflasyon oranlarını çeker ve veritabanına kaydeder.
    ///
    /// Veri Akışı:
    /// 1. Her gün 09:00'da TCMB XML API'den USD ve EUR kurları çekilir
    /// 2. Aylık periyotlarla TÜİK'ten enflasyon verisi güncellenir
    /// 3. Çekilen veriler veritabanına persist edilir
    /// 4. Hareketli ortalama ve trend analizleri hesaplanır
    /// </summary>
    public class MacroEconomyIntegrationService : IMacroEconomyIntegrationService
    {
        private readonly IExchangeRateRepository _exchangeRateRepo;
        private readonly IGenericRepository<InflationData> _inflationRepo;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MacroEconomyIntegrationService> _logger;

        // TCMB Günlük Döviz Kurları XML Endpoint'i
        private const string TCMB_DAILY_RATES_URL = "https://www.tcmb.gov.tr/kurlar/today.xml";
        private const string TCMB_ARCHIVE_URL = "https://www.tcmb.gov.tr/kurlar/{0}/{1}.xml"; // year/monthday

        // Retry ayarları
        private const int MAX_RETRY_COUNT = 3;
        private const int INITIAL_RETRY_DELAY_MS = 1000;

        public MacroEconomyIntegrationService(
            IExchangeRateRepository exchangeRateRepo,
            IGenericRepository<InflationData> inflationRepo,
            IHttpClientFactory httpClientFactory,
            ILogger<MacroEconomyIntegrationService> logger)
        {
            _exchangeRateRepo = exchangeRateRepo;
            _inflationRepo = inflationRepo;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        #region Döviz Kuru İşlemleri

        /// <summary>
        /// Belirtilen döviz cinsi için güncel kuru getirir.
        /// Önce veritabanından bugünün kurunu kontrol eder.
        /// Yoksa TCMB'den çekip kaydeder.
        /// </summary>
        public async Task<ExchangeRate> GetCurrentExchangeRateAsync(CurrencyType currency)
        {
            // Önce veritabanında bugünün kuru var mı kontrol et
            var todayRate = await _exchangeRateRepo.GetLatestRateAsync(currency);

            if (todayRate != null && todayRate.RateDate.Date == DateTime.UtcNow.Date)
            {
                return todayRate;
            }

            // Veritabanında güncel kur yoksa, TCMB'den çek
            _logger.LogInformation("Veritabanında güncel kur bulunamadı, TCMB'den çekiliyor...");
            await SyncDailyRatesAsync();

            // Tekrar sorgula
            todayRate = await _exchangeRateRepo.GetLatestRateAsync(currency);

            if (todayRate == null)
            {
                _logger.LogWarning("TCMB'den kur çekilemedi, son bilinen kur kullanılıyor.");
                todayRate = await _exchangeRateRepo.GetLatestRateAsync(currency);
            }

            return todayRate ?? throw new InvalidOperationException(
                $"Hiçbir {currency} kuru bulunamadı. Lütfen internet bağlantınızı kontrol edin.");
        }

        /// <summary>
        /// Belirtilen tarih aralığında döviz kuru geçmişini getirir.
        /// Trend analizi ve hareketli ortalama hesaplamaları için kullanılır.
        /// </summary>
        public async Task<List<ExchangeRate>> GetExchangeRateHistoryAsync(
            CurrencyType currency, DateTime from, DateTime to)
        {
            var rates = await _exchangeRateRepo.GetRateHistoryAsync(
                currency, (int)(to - from).TotalDays);
            return rates.Where(r => r.RateDate >= from && r.RateDate <= to).ToList();
        }

        /// <summary>
        /// TCMB XML API'den günlük USD ve EUR kurlarını çeker ve veritabanına kaydeder.
        ///
        /// TCMB XML Yapısı:
        /// <Tarih_Date>
        ///   <Currency CurrencyCode="USD">
        ///     <ForexBuying>34.5678</ForexBuying>
        ///     <ForexSelling>34.6789</ForexSelling>
        ///   </Currency>
        /// </Tarih_Date>
        ///
        /// Hafta sonu ve resmi tatillerde TCMB kur yayınlamaz,
        /// bu durumda en son bilinen kur kullanılır.
        /// </summary>
        public async Task SyncDailyRatesAsync()
        {
            _logger.LogInformation("TCMB günlük döviz kuru senkronizasyonu başlatılıyor...");

            try
            {
                var xmlContent = await FetchWithRetryAsync(TCMB_DAILY_RATES_URL);

                if (string.IsNullOrEmpty(xmlContent))
                {
                    _logger.LogError("TCMB'den boş yanıt alındı.");
                    return;
                }

                var doc = XDocument.Parse(xmlContent);
                var root = doc.Root;

                if (root == null)
                {
                    _logger.LogError("TCMB XML parse hatası: root element bulunamadı.");
                    return;
                }

                // Tarih bilgisini al
                var dateAttr = root.Attribute("Tarih");
                var rateDate = DateTime.UtcNow.Date;
                if (dateAttr != null)
                {
                    // Format: "25.03.2026"
                    DateTime.TryParseExact(dateAttr.Value, "dd.MM.yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out rateDate);
                }

                // Tüm döviz cinsleri için kurları çek
                var currencies = root.Elements("Currency");

                foreach (var currencyElement in currencies)
                {
                    var currencyCode = currencyElement.Attribute("CurrencyCode")?.Value;

                    if (currencyCode != "USD" && currencyCode != "EUR")
                        continue;

                    var forexBuyingStr = currencyElement.Element("ForexBuying")?.Value;
                    var forexSellingStr = currencyElement.Element("ForexSelling")?.Value;

                    if (string.IsNullOrEmpty(forexBuyingStr) || string.IsNullOrEmpty(forexSellingStr))
                        continue;

                    // TCMB ondalık ayracı nokta kullanır
                    var buyRate = ParseDecimal(forexBuyingStr);
                    var sellRate = ParseDecimal(forexSellingStr);

                    if (buyRate <= 0 || sellRate <= 0)
                        continue;

                    // Veritabanında bu tarih için kayıt var mı kontrol et
                    var existingRate = await _exchangeRateRepo.GetRateByDateAsync(
                        currencyCode == "USD" ? CurrencyType.USD : CurrencyType.EUR,
                        rateDate);

                    if (existingRate != null)
                    {
                        _logger.LogInformation($"{currencyCode} kuru {rateDate:dd.MM.yyyy} için zaten mevcut.");
                        continue;
                    }

                    // Yeni kur kaydı oluştur
                    var exchangeRate = new ExchangeRate
                    {
                        RateDate = rateDate,
                        USDRate = currencyCode == "USD" ? buyRate : 0,
                        EURRate = currencyCode == "EUR" ? buyRate : 0,
                        USDSellRate = currencyCode == "USD" ? sellRate : null,
                        EURSellRate = currencyCode == "EUR" ? sellRate : null,
                        Source = "TCMB",
                        CreatedAt = DateTime.UtcNow
                    };

                    await _exchangeRateRepo.AddAsync(exchangeRate);
                    _logger.LogInformation(
                        $"TCMB {currencyCode} kuru kaydedildi: Alış={buyRate:F4}, Satış={sellRate:F4} ({rateDate:dd.MM.yyyy})");
                }

                await _exchangeRateRepo.SaveChangesAsync();
                _logger.LogInformation("TCMB günlük kur senkronizasyonu tamamlandı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TCMB kur senkronizasyonunda hata oluştu.");
                throw;
            }
        }

        #endregion

        #region Enflasyon Veri İşlemleri

        /// <summary>
        /// En güncel enflasyon verisini getirir.
        /// TÜİK her ayın 3'ünde bir önceki ayın TÜFE verisini yayınlar.
        /// </summary>
        public async Task<InflationData> GetLatestInflationDataAsync()
        {
            var allData = await _inflationRepo.GetAllAsync();
            var latest = allData
                .OrderByDescending(i => i.Year)
                .ThenByDescending(i => i.Month)
                .FirstOrDefault();

            if (latest == null)
            {
                // Varsayılan/fallback enflasyon verisi
                _logger.LogWarning("Enflasyon verisi bulunamadı, varsayılan değerler kullanılıyor.");
                return new InflationData
                {
                    Year = DateTime.UtcNow.Year,
                    Month = DateTime.UtcNow.Month > 1 ? DateTime.UtcNow.Month - 1 : 12,
                    MonthlyRate = 2.5m,    // Varsayılan aylık enflasyon
                    YearlyRate = 45.0m,    // Varsayılan yıllık enflasyon
                    Source = "Varsayılan"
                };
            }

            return latest;
        }

        /// <summary>
        /// Belirtilen ay sayısı kadar geriye dönük enflasyon geçmişini getirir.
        /// Enflasyon trendi analizi ve satın alma gücü hesaplaması için kullanılır.
        /// </summary>
        public async Task<List<InflationData>> GetInflationHistoryAsync(int months)
        {
            var allData = await _inflationRepo.GetAllAsync();
            return allData
                .OrderByDescending(i => i.Year)
                .ThenByDescending(i => i.Month)
                .Take(months)
                .ToList();
        }

        /// <summary>
        /// Enflasyon verilerini günceller.
        ///
        /// Gerçek implementasyonda TÜİK EVDS API kullanılır.
        /// Bu versiyon, kullanıcının manuel giriş yapması veya
        /// bir veri sağlayıcıdan çekilmesi için altyapı sağlar.
        ///
        /// TÜİK EVDS API Endpoint (örnek):
        /// https://evds2.tcmb.gov.tr/service/evds/series=TP.FG.J0
        /// </summary>
        public async Task SyncInflationDataAsync()
        {
            _logger.LogInformation("Enflasyon veri senkronizasyonu başlatılıyor...");

            try
            {
                // EVDS (Elektronik Veri Dağıtım Sistemi) API'den enflasyon çekme
                // Not: EVDS API key gerektirir. Burada yapısal çatı hazırlanmıştır.
                var client = _httpClientFactory.CreateClient("EVDS");

                // TCMB EVDS API'den TÜFE verisi çekme denemesi
                // Seri kodu: TP.FG.J0 (TÜFE Genel)
                var evdsUrl = "https://evds2.tcmb.gov.tr/service/evds/" +
                    $"series=TP.FG.J0&startDate=01-01-{DateTime.UtcNow.Year}" +
                    $"&endDate=31-12-{DateTime.UtcNow.Year}&type=json";

                try
                {
                    var response = await client.GetStringAsync(evdsUrl);
                    // JSON parse ve kaydetme işlemi
                    _logger.LogInformation("EVDS'den enflasyon verisi başarıyla çekildi.");
                }
                catch (HttpRequestException)
                {
                    _logger.LogWarning(
                        "EVDS API'ye ulaşılamadı. Enflasyon verisi manuel olarak güncellenmelidir.");
                }

                await _inflationRepo.SaveChangesAsync();
                _logger.LogInformation("Enflasyon veri senkronizasyonu tamamlandı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Enflasyon senkronizasyonunda hata oluştu.");
                throw;
            }
        }

        #endregion

        #region Yardımcı Metodlar

        /// <summary>
        /// Belirtilen URL'den HTTP GET isteği ile veri çeker.
        /// Başarısız olursa üstel geri çekilme (exponential backoff) ile yeniden dener.
        ///
        /// Retry Stratejisi:
        /// - 1. deneme: anında
        /// - 2. deneme: 1 saniye sonra
        /// - 3. deneme: 2 saniye sonra
        /// - 4. deneme: 4 saniye sonra
        /// </summary>
        private async Task<string?> FetchWithRetryAsync(string url)
        {
            var client = _httpClientFactory.CreateClient("TCMB");
            var delayMs = INITIAL_RETRY_DELAY_MS;

            for (int attempt = 0; attempt <= MAX_RETRY_COUNT; attempt++)
            {
                try
                {
                    if (attempt > 0)
                    {
                        _logger.LogWarning(
                            $"TCMB isteği yeniden deneniyor (Deneme {attempt + 1}/{MAX_RETRY_COUNT + 1})...");
                        await Task.Delay(delayMs);
                        delayMs *= 2; // Üstel geri çekilme
                    }

                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning(ex,
                        $"TCMB isteği başarısız (Deneme {attempt + 1}/{MAX_RETRY_COUNT + 1})");

                    if (attempt == MAX_RETRY_COUNT)
                    {
                        _logger.LogError("TCMB'ye tüm bağlantı denemeleri başarısız oldu.");
                        return null;
                    }
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogWarning(ex, "TCMB isteği zaman aşımına uğradı.");
                    if (attempt == MAX_RETRY_COUNT) return null;
                }
            }

            return null;
        }

        /// <summary>
        /// TCMB XML'deki ondalık değerleri parse eder.
        /// TCMB nokta (.) kullanır ancak bazı durumlarda virgül de olabilir.
        /// </summary>
        private static decimal ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            value = value.Trim();

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;

            // Virgül ile dene (Türk formatı fallback)
            if (decimal.TryParse(value, NumberStyles.Any, new CultureInfo("tr-TR"), out result))
                return result;

            return 0;
        }

        #endregion
    }
}
