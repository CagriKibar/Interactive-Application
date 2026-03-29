using FinancialERP.Business.Services.Abstract;
using FinancialERP.DataAccess.BarsoftIntegration;
using FinancialERP.DataAccess.Repositories.Abstract;
using FinancialERP.Entity.DTOs;
using FinancialERP.Entity.Enums;
using FinancialERP.Entity.Models;
using Microsoft.Extensions.Logging;

namespace FinancialERP.Business.Services.Concrete
{
    /// <summary>
    /// Finansal Analiz ve Öneri Motoru (Decision Support Engine)
    ///
    /// Bu servis, işletmenin tüm finansal verilerini harmanlayarak:
    /// 1. Kâr marjı erimesini tespit eder (döviz kuru etkisi)
    /// 2. Stok optimizasyonu önerileri üretir (nakit + kur trendi + satış verisi)
    /// 3. Nakit akışı sağlık skoru hesaplar (0-100)
    /// 4. Yönetici dashboard özeti oluşturur
    ///
    /// Veri Kaynakları:
    /// - Barsoft ERP (ürün, satış, stok verileri)
    /// - TCMB (döviz kurları)
    /// - TÜİK (enflasyon verileri)
    /// - İşletme verileri (banka, kredi kartı, giderler)
    /// </summary>
    public class FinancialAnalysisAndRecommendationService : IFinancialAnalysisService
    {
        private readonly IExchangeRateRepository _exchangeRateRepo;
        private readonly IGenericRepository<InflationData> _inflationRepo;
        private readonly IGenericRepository<CreditCard> _creditCardRepo;
        private readonly IGenericRepository<BankAccount> _bankAccountRepo;
        private readonly IGenericRepository<Expense> _expenseRepo;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<DailySales> _dailySalesRepo;
        private readonly IBarsoftDataService _barsoftService;
        private readonly IMacroEconomyIntegrationService _macroService;
        private readonly ILogger<FinancialAnalysisAndRecommendationService> _logger;

        // Hedef kâr marjı oranı (varsayılan %25)
        private const decimal TARGET_PROFIT_MARGIN = 0.25m;
        // Kâr marjı erimesi uyarı eşiği (%5)
        private const decimal MARGIN_EROSION_WARNING_THRESHOLD = 5m;
        // Kâr marjı erimesi kritik eşiği (%10)
        private const decimal MARGIN_EROSION_CRITICAL_THRESHOLD = 10m;
        // Stok tükenme uyarı eşiği (gün)
        private const int STOCK_DAYS_WARNING_THRESHOLD = 15;

        public FinancialAnalysisAndRecommendationService(
            IExchangeRateRepository exchangeRateRepo,
            IGenericRepository<InflationData> inflationRepo,
            IGenericRepository<CreditCard> creditCardRepo,
            IGenericRepository<BankAccount> bankAccountRepo,
            IGenericRepository<Expense> expenseRepo,
            IGenericRepository<Product> productRepo,
            IGenericRepository<DailySales> dailySalesRepo,
            IBarsoftDataService barsoftService,
            IMacroEconomyIntegrationService macroService,
            ILogger<FinancialAnalysisAndRecommendationService> logger)
        {
            _exchangeRateRepo = exchangeRateRepo;
            _inflationRepo = inflationRepo;
            _creditCardRepo = creditCardRepo;
            _bankAccountRepo = bankAccountRepo;
            _expenseRepo = expenseRepo;
            _productRepo = productRepo;
            _dailySalesRepo = dailySalesRepo;
            _barsoftService = barsoftService;
            _macroService = macroService;
            _logger = logger;
        }

        #region 1. Kâr Marjı Analizi

        /// <summary>
        /// Kâr Marjı Erimesi Analiz Algoritması
        ///
        /// Mantık:
        /// 1. Barsoft'tan tüm aktif ürünleri çek
        /// 2. Güncel ve 30 gün önceki USD/TRY kurlarını al
        /// 3. USD bazlı maliyeti olan her ürün için:
        ///    - Önceki maliyet = CostPriceInUSD × eskiKur
        ///    - Güncel maliyet = CostPriceInUSD × güncelKur
        ///    - Marj erimesi = ((güncelMaliyet - eskiMaliyet) / eskiMaliyet) × 100
        /// 4. Erime > %10 → Kritik uyarı
        /// 5. Erime > %5  → Uyarı
        /// 6. Önerilen yeni fiyat = CostPriceInUSD × güncelKur × (1 + hedefMarj)
        /// </summary>
        public async Task<List<ProfitMarginAnalysis>> AnalyzeProfitMarginsAsync()
        {
            _logger.LogInformation("Kâr marjı analizi başlatılıyor...");
            var results = new List<ProfitMarginAnalysis>();

            try
            {
                // Güncel ve 30 gün önceki döviz kurlarını al
                var currentRate = await _macroService.GetCurrentExchangeRateAsync(CurrencyType.USD);
                var historicRates = await _macroService.GetExchangeRateHistoryAsync(
                    CurrencyType.USD,
                    DateTime.UtcNow.AddDays(-30),
                    DateTime.UtcNow);

                var previousRate = historicRates.LastOrDefault() ?? currentRate;
                var currentUsdRate = currentRate.USDRate;
                var previousUsdRate = previousRate.USDRate;

                // Kur değişim oranı
                var rateChangePercent = previousUsdRate > 0
                    ? ((currentUsdRate - previousUsdRate) / previousUsdRate) * 100
                    : 0;

                // Tüm ürünleri al
                var products = await _productRepo.GetAllAsync();

                foreach (var product in products.Where(p => p.CostPriceInUSD.HasValue && p.CostPriceInUSD > 0))
                {
                    var costInUsd = product.CostPriceInUSD!.Value;

                    // Önceki ve güncel TRY maliyet hesaplaması
                    var previousCostTRY = costInUsd * previousUsdRate;
                    var currentCostTRY = costInUsd * currentUsdRate;

                    // Önceki ve güncel kâr marjları
                    var previousMargin = product.SalePrice > 0
                        ? ((product.SalePrice - previousCostTRY) / product.SalePrice) * 100
                        : 0;
                    var currentMargin = product.SalePrice > 0
                        ? ((product.SalePrice - currentCostTRY) / product.SalePrice) * 100
                        : 0;

                    // Marj erimesi
                    var marginErosion = previousMargin - currentMargin;

                    if (marginErosion <= 0) continue; // Marj artmış, sorun yok

                    // Önerilen yeni fiyat: maliyetin üzerine hedef marj ekle
                    var suggestedPrice = currentCostTRY * (1 + TARGET_PROFIT_MARGIN);

                    // Uyarı mesajı oluştur
                    var severity = marginErosion >= MARGIN_EROSION_CRITICAL_THRESHOLD
                        ? "KRİTİK" : "UYARI";

                    var analysis = new ProfitMarginAnalysis
                    {
                        ProductCode = product.ProductCode ?? "",
                        ProductName = product.Name,
                        CurrentSalePrice = product.SalePrice,
                        CurrentCostPrice = currentCostTRY,
                        CostPriceInUSD = costInUsd,
                        PreviousMarginPercent = Math.Round(previousMargin, 2),
                        CurrentMarginPercent = Math.Round(currentMargin, 2),
                        MarginChangePercent = Math.Round(marginErosion, 2),
                        SuggestedNewPrice = Math.Round(suggestedPrice, 2),
                        CurrencyImpactNote = $"[{severity}] {product.Name} ürününün satış fiyatı " +
                            $"aynı kalmasına rağmen artan döviz kuru sebebiyle kâr marjınız " +
                            $"son 1 ayda %{marginErosion:F1} eridi. " +
                            $"Fiyat güncellemesi önerilir. " +
                            $"Önerilen yeni fiyat: {suggestedPrice:N2} TL"
                    };

                    results.Add(analysis);
                }

                _logger.LogInformation($"Kâr marjı analizi tamamlandı. {results.Count} ürün uyarısı üretildi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kâr marjı analizinde hata oluştu.");
            }

            return results.OrderByDescending(r => r.MarginChangePercent).ToList();
        }

        #endregion

        #region 2. Stok Optimizasyonu

        /// <summary>
        /// Stok/Maliyet Optimizasyon Algoritması
        ///
        /// Mantık:
        /// 1. Tüm kredi kartlarının yaklaşan ödemelerini topla
        /// 2. Tüm banka bakiyelerini topla → net nakit pozisyonu
        /// 3. Son 7 ve 30 günlük döviz kuru hareketli ortalamalarını hesapla
        /// 4. Eğer kur trendi yukarı yönlü VE nakit pozisyonu pozitifse:
        ///    - Barsoft'tan en çok satan ürünleri al
        ///    - Her ürün için kalan stok gününü hesapla
        ///    - Stok < 15 gün ise stoklama önerisi üret
        /// 5. Türkçe öneri mesajları oluştur
        /// </summary>
        public async Task<List<StockOptimizationSuggestion>> AnalyzeStockOptimizationAsync()
        {
            _logger.LogInformation("Stok optimizasyonu analizi başlatılıyor...");
            var suggestions = new List<StockOptimizationSuggestion>();

            try
            {
                // 1. Kredi kartı yaklaşan ödemeleri
                var creditCards = await _creditCardRepo.GetAllAsync();
                var upcomingCCPayments = creditCards.Sum(cc => cc.CurrentSpending);

                // 2. Banka bakiyeleri (TRY'ye çevirerek)
                var bankAccounts = await _bankAccountRepo.GetAllAsync();
                var currentRate = await _macroService.GetCurrentExchangeRateAsync(CurrencyType.USD);
                var eurRate = await _macroService.GetCurrentExchangeRateAsync(CurrencyType.EUR);

                decimal totalBankBalanceTRY = 0;
                foreach (var account in bankAccounts)
                {
                    totalBankBalanceTRY += account.Currency switch
                    {
                        CurrencyType.USD => account.Balance * currentRate.USDRate,
                        CurrencyType.EUR => account.Balance * eurRate.EURRate,
                        _ => account.Balance
                    };
                }

                // Net nakit pozisyonu
                var netCashPosition = totalBankBalanceTRY - upcomingCCPayments;

                // 3. Döviz kuru trend analizi (7 günlük vs 30 günlük hareketli ortalama)
                var rateHistory = await _macroService.GetExchangeRateHistoryAsync(
                    CurrencyType.USD, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);

                var last7Days = rateHistory.Take(7).ToList();
                var last30Days = rateHistory.ToList();

                var avg7Day = last7Days.Any() ? last7Days.Average(r => r.USDRate) : 0;
                var avg30Day = last30Days.Any() ? last30Days.Average(r => r.USDRate) : 0;

                // Trend yönü: 7 günlük ortalama > 30 günlük ortalama ise yukarı yönlü
                var trendDirection = avg7Day > avg30Day ? "Yukarı" : avg7Day < avg30Day ? "Aşağı" : "Yatay";
                var isUpwardTrend = avg7Day > avg30Day;

                // 4. Nakit fazlası var ve kur yukarı gidiyorsa → stok önerisi
                if (netCashPosition > 0 && isUpwardTrend)
                {
                    // Son 30 günün en çok satan ürünlerini al
                    var topProducts = await _barsoftService.GetTopSellingProductsAsync(30, 10);
                    var last30DaysSales = await _barsoftService.GetSalesInRangeAsync(
                        DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);

                    foreach (var product in topProducts)
                    {
                        // Ürünün günlük ortalama satışını hesapla
                        var productSales = last30DaysSales
                            .Where(s => s.ProductCode == product.ProductCode)
                            .Sum(s => s.Quantity);
                        var avgDailySales = productSales / 30m;

                        // Kalan stok gün sayısı
                        var daysOfStockLeft = avgDailySales > 0
                            ? (int)(product.CurrentStock / avgDailySales)
                            : 999;

                        if (daysOfStockLeft < STOCK_DAYS_WARNING_THRESHOLD)
                        {
                            // Önerilen sipariş miktarı: 30 günlük satışı karşılayacak kadar
                            var suggestedQty = (avgDailySales * 30) - product.CurrentStock;
                            if (suggestedQty < 0) suggestedQty = 0;

                            var estimatedCost = suggestedQty * product.CostPrice;

                            suggestions.Add(new StockOptimizationSuggestion
                            {
                                ProductCode = product.ProductCode,
                                ProductName = product.ProductName,
                                CurrentStock = product.CurrentStock,
                                AverageDailySales = Math.Round(avgDailySales, 1),
                                EstimatedDaysUntilStockout = daysOfStockLeft,
                                SuggestedOrderQuantity = Math.Round(suggestedQty, 0),
                                EstimatedOrderCost = Math.Round(estimatedCost, 2),
                                Rationale = $"Önümüzdeki ay kredi kartı ödemeleriniz toplam " +
                                    $"{upcomingCCPayments:N2} TL. Bankadaki nakdiniz {totalBankBalanceTRY:N2} TL. " +
                                    $"Nakit fazlanız var ve Dolar kurunda yukarı yönlü hareket tespit edildi. " +
                                    $"Barsoft verilerine göre en çok satan {product.ProductName} ürününün " +
                                    $"stoğunu artırmak mantıklı bir yatırım olabilir."
                            });
                        }
                    }
                }

                _logger.LogInformation(
                    $"Stok optimizasyonu tamamlandı. Trend: {trendDirection}, " +
                    $"Net Nakit: {netCashPosition:N2} TL, {suggestions.Count} öneri üretildi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stok optimizasyonu analizinde hata oluştu.");
            }

            return suggestions;
        }

        #endregion

        #region 3. Nakit Akışı Sağlık Analizi

        /// <summary>
        /// Nakit Akışı Sağlık Skoru Algoritması (0-100)
        ///
        /// Skorlama:
        /// - Başlangıç skoru: 100
        /// - Net nakit negatifse: -40 puan
        /// - Yaklaşan ödemeler > bakiyenin %80'i ise: -20 puan
        /// - Aylık giderler > aylık gelirin %90'ı ise: -20 puan
        /// - Enflasyon > %5 aylık ise: -10 puan
        /// - Döviz artışı > %5 aylık ise: -10 puan
        ///
        /// Reel Değer Hesaplaması:
        /// - Reel Değer = Nominal Değer / (1 + yıllık enflasyon oranı)
        /// - USD Karşılığı = TRY Bakiye / güncel USD kuru
        /// </summary>
        public async Task<CashFlowReport> AnalyzeCashFlowHealthAsync()
        {
            _logger.LogInformation("Nakit akışı sağlık analizi başlatılıyor...");

            var report = new CashFlowReport();
            var recommendations = new List<FinancialRecommendation>();

            try
            {
                // Verileri topla
                var bankAccounts = await _bankAccountRepo.GetAllAsync();
                var creditCards = await _creditCardRepo.GetAllAsync();
                var expenses = await _expenseRepo.GetAllAsync();
                var currentUsdRate = await _macroService.GetCurrentExchangeRateAsync(CurrencyType.USD);
                var currentEurRate = await _macroService.GetCurrentExchangeRateAsync(CurrencyType.EUR);
                var inflation = await _macroService.GetLatestInflationDataAsync();

                // 1. Toplam banka bakiyesi (TRY'ye çevir)
                decimal totalBalanceTRY = 0;
                foreach (var account in bankAccounts)
                {
                    totalBalanceTRY += account.Currency switch
                    {
                        CurrencyType.USD => account.Balance * currentUsdRate.USDRate,
                        CurrencyType.EUR => account.Balance * currentEurRate.EURRate,
                        _ => account.Balance
                    };
                }
                report.TotalBankBalance = Math.Round(totalBalanceTRY, 2);

                // 2. USD karşılığı
                report.TotalBankBalanceInUSD = currentUsdRate.USDRate > 0
                    ? Math.Round(totalBalanceTRY / currentUsdRate.USDRate, 2)
                    : 0;

                // 3. Toplam kredi kartı borcu
                report.TotalCreditCardDebt = creditCards.Sum(cc => cc.CurrentSpending);

                // 4. Yaklaşan aylık giderler (tekrarlayan giderler)
                var recurringExpenses = expenses.Where(e => e.IsRecurring);
                report.UpcomingMonthlyExpenses = recurringExpenses.Sum(e => e.Amount)
                    + creditCards.Sum(cc => cc.MinimumPayment);

                // 5. Aylık tahmini gelir (son 30 gün Barsoft cirosu)
                var dailyRevenue = await _barsoftService.GetDailyRevenueAsync(DateTime.UtcNow);
                report.ProjectedMonthlyRevenue = dailyRevenue * 30;

                // 6. Net nakit pozisyonu
                report.NetCashPosition = report.TotalBankBalance - report.TotalCreditCardDebt;

                // 7. Enflasyona göre reel değer
                var yearlyInflation = inflation.YearlyRate / 100m;
                report.InflationAdjustedValue = yearlyInflation > 0
                    ? Math.Round(report.NetCashPosition / (1 + yearlyInflation), 2)
                    : report.NetCashPosition;
                report.RealPurchasingPowerTRY = report.InflationAdjustedValue;

                // === SKOR HESAPLAMASI ===
                decimal score = 100;

                // Net nakit negatifse
                if (report.NetCashPosition < 0)
                {
                    score -= 40;
                    recommendations.Add(new FinancialRecommendation
                    {
                        Title = "Kritik: Negatif Nakit Pozisyonu",
                        Message = $"Net nakit pozisyonunuz {report.NetCashPosition:N2} TL ile negatif. " +
                            "Acil nakit girişi sağlanmalı veya borçlar yeniden yapılandırılmalıdır.",
                        Severity = AlertSeverity.Critical,
                        Category = AlertCategory.CashFlow,
                        ActionItems = new List<string>
                        {
                            "Kredi kartı borçlarını taksitlendirin",
                            "Vadesi gelen alacakları tahsil edin",
                            "Gereksiz gider kalemlerini gözden geçirin"
                        }
                    });
                }

                // Yaklaşan ödemeler bakiyenin %80'inden fazlaysa
                if (report.TotalBankBalance > 0 &&
                    report.UpcomingMonthlyExpenses > report.TotalBankBalance * 0.8m)
                {
                    score -= 20;
                    recommendations.Add(new FinancialRecommendation
                    {
                        Title = "Uyarı: Ödeme/Bakiye Oranı Yüksek",
                        Message = $"Yaklaşan ödemeleriniz ({report.UpcomingMonthlyExpenses:N2} TL) " +
                            $"banka bakiyenizin ({report.TotalBankBalance:N2} TL) %80'ini aşıyor.",
                        Severity = AlertSeverity.Warning,
                        Category = AlertCategory.CashFlow,
                        ActionItems = new List<string>
                        {
                            "Ödeme takvimini gözden geçirin",
                            "Öncelikli ödemeleri belirleyin",
                            "Ek gelir kaynakları değerlendirin"
                        }
                    });
                }

                // Aylık giderler > gelirin %90'ı
                if (report.ProjectedMonthlyRevenue > 0 &&
                    report.UpcomingMonthlyExpenses > report.ProjectedMonthlyRevenue * 0.9m)
                {
                    score -= 20;
                    recommendations.Add(new FinancialRecommendation
                    {
                        Title = "Uyarı: Gider/Gelir Dengesi Bozuk",
                        Message = $"Aylık giderleriniz ({report.UpcomingMonthlyExpenses:N2} TL) " +
                            $"tahmini gelirinizin ({report.ProjectedMonthlyRevenue:N2} TL) %90'ını aşıyor.",
                        Severity = AlertSeverity.Warning,
                        Category = AlertCategory.FinancingAdvice,
                        ActionItems = new List<string>
                        {
                            "Sabit giderleri minimize edin",
                            "Fiyat politikasını revize edin",
                            "Satış hacmini artıracak kampanyalar planlayın"
                        }
                    });
                }

                // Enflasyon etkisi
                if (inflation.MonthlyRate > 5)
                {
                    score -= 10;
                    recommendations.Add(new FinancialRecommendation
                    {
                        Title = "Enflasyon Etkisi Yüksek",
                        Message = $"Aylık enflasyon %{inflation.MonthlyRate:F1} seviyesinde. " +
                            $"Paranızın reel satın alma gücü azalıyor. " +
                            $"Nominalde {report.NetCashPosition:N2} TL olan nakdinizin " +
                            $"reel değeri {report.InflationAdjustedValue:N2} TL.",
                        Severity = AlertSeverity.Warning,
                        Category = AlertCategory.InflationImpact
                    });
                }

                // Skor sınırlama
                score = Math.Max(0, Math.Min(100, score));

                report.HealthStatus = score switch
                {
                    >= 70 => CashFlowHealth.Healthy,
                    >= 50 => CashFlowHealth.Moderate,
                    >= 30 => CashFlowHealth.Warning,
                    _ => CashFlowHealth.Critical
                };

                report.Recommendations = recommendations;

                _logger.LogInformation(
                    $"Nakit akışı analizi tamamlandı. Skor: {score}, Durum: {report.HealthStatus}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nakit akışı analizinde hata oluştu.");
            }

            return report;
        }

        #endregion

        #region 4. Makroekonomik Özet

        public async Task<MacroEconomicSnapshot> GetMacroEconomicSummaryAsync()
        {
            var usdRate = await _macroService.GetCurrentExchangeRateAsync(CurrencyType.USD);
            var eurRate = await _macroService.GetCurrentExchangeRateAsync(CurrencyType.EUR);
            var inflation = await _macroService.GetLatestInflationDataAsync();

            // 30 günlük değişim hesapla
            var usdHistory = await _macroService.GetExchangeRateHistoryAsync(
                CurrencyType.USD, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
            var oldestUsd = usdHistory.LastOrDefault();
            var usdChange = oldestUsd != null && oldestUsd.USDRate > 0
                ? ((usdRate.USDRate - oldestUsd.USDRate) / oldestUsd.USDRate) * 100
                : 0;

            var eurHistory = await _macroService.GetExchangeRateHistoryAsync(
                CurrencyType.EUR, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
            var oldestEur = eurHistory.LastOrDefault();
            var eurChange = oldestEur != null && oldestEur.EURRate > 0
                ? ((eurRate.EURRate - oldestEur.EURRate) / oldestEur.EURRate) * 100
                : 0;

            var avg7 = usdHistory.Take(7).Average(r => r.USDRate);
            var avg30 = usdHistory.Any() ? usdHistory.Average(r => r.USDRate) : avg7;
            var trend = avg7 > avg30 ? "Yukarı Yönlü" : avg7 < avg30 ? "Aşağı Yönlü" : "Yatay";

            return new MacroEconomicSnapshot
            {
                UsdBuyRate = usdRate.USDRate,
                UsdSellRate = usdRate.USDSellRate ?? usdRate.USDRate,
                EurBuyRate = eurRate.EURRate,
                EurSellRate = eurRate.EURSellRate ?? eurRate.EURRate,
                UsdDailyChange = Math.Round(usdChange, 2),
                EurDailyChange = Math.Round(eurChange, 2),
                MonthlyInflation = inflation.MonthlyRate,
                AnnualInflation = inflation.YearlyRate,
                LastUpdated = usdRate.CreatedAt,
                TrendDirection = trend
            };
        }

        #endregion

        #region 5. Yönetici Dashboard

        public async Task<DashboardSummary> GenerateExecutiveDashboardAsync()
        {
            _logger.LogInformation("Yönetici dashboard özeti oluşturuluyor...");

            var profitMargins = await AnalyzeProfitMarginsAsync();
            var stockAlerts = await AnalyzeStockOptimizationAsync();
            var cashFlow = await AnalyzeCashFlowHealthAsync();
            var macro = await GetMacroEconomicSummaryAsync();
            var notifications = await GenerateSmartNotificationsAsync();

            var todayRevenue = await _barsoftService.GetDailyRevenueAsync(DateTime.UtcNow);
            var todaySales = await _barsoftService.GetDailySalesAsync(DateTime.UtcNow);
            var todayProfit = todaySales.Sum(s => s.ProfitAmount);

            return new DashboardSummary
            {
                TodayRevenue = todayRevenue,
                TodayProfit = todayProfit,
                UsdRate = macro.UsdBuyRate,
                EurRate = macro.EurBuyRate,
                UsdChangePercent = macro.UsdDailyChange,
                EurChangePercent = macro.EurDailyChange,
                MonthlyInflationRate = macro.MonthlyInflation,
                AnnualInflationRate = macro.AnnualInflation,
                CashFlow = cashFlow,
                ErodedMargins = profitMargins,
                StockAlerts = stockAlerts,
                Notifications = notifications
            };
        }

        #endregion

        #region 6. Akıllı Bildirim Üretici

        public async Task<List<FinancialRecommendation>> GenerateSmartNotificationsAsync()
        {
            var notifications = new List<FinancialRecommendation>();

            // Kâr marjı uyarıları
            var margins = await AnalyzeProfitMarginsAsync();
            foreach (var m in margins.Where(x => x.MarginChangePercent >= MARGIN_EROSION_WARNING_THRESHOLD))
            {
                notifications.Add(new FinancialRecommendation
                {
                    Title = $"Kâr Marjı Uyarısı: {m.ProductName}",
                    Message = m.CurrencyImpactNote,
                    Severity = m.MarginChangePercent >= MARGIN_EROSION_CRITICAL_THRESHOLD
                        ? AlertSeverity.Critical : AlertSeverity.Warning,
                    Category = AlertCategory.ProfitMargin,
                    ImpactAmount = m.SuggestedNewPrice - m.CurrentSalePrice,
                    ImpactPercentage = $"%{m.MarginChangePercent:F1}",
                    ActionItems = new List<string>
                    {
                        $"Satış fiyatını {m.SuggestedNewPrice:N2} TL olarak güncelleyin",
                        "Alternatif tedarikçi araştırın",
                        "TRY bazlı tedarikçiye geçiş değerlendirin"
                    }
                });
            }

            // Nakit akışı uyarıları
            var cashFlow = await AnalyzeCashFlowHealthAsync();
            notifications.AddRange(cashFlow.Recommendations);

            // Stok optimizasyon fırsatları
            var stockSuggestions = await AnalyzeStockOptimizationAsync();
            foreach (var s in stockSuggestions)
            {
                notifications.Add(new FinancialRecommendation
                {
                    Title = $"Stok Fırsatı: {s.ProductName}",
                    Message = s.Rationale,
                    Severity = AlertSeverity.Opportunity,
                    Category = AlertCategory.StockOptimization,
                    ImpactAmount = s.EstimatedOrderCost,
                    ActionItems = new List<string>
                    {
                        $"{s.SuggestedOrderQuantity:N0} adet sipariş verin",
                        $"Tahmini maliyet: {s.EstimatedOrderCost:N2} TL",
                        $"Stok {s.EstimatedDaysUntilStockout} gün içinde tükenecek"
                    }
                });
            }

            return notifications.OrderByDescending(n => n.Severity).ToList();
        }

        #endregion
    }
}
