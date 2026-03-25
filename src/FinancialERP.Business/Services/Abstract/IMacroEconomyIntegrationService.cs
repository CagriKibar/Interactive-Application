using FinancialERP.Entity.Enums;
using FinancialERP.Entity.Models;

namespace FinancialERP.Business.Services.Abstract
{
    /// <summary>
    /// Makroekonomik veri entegrasyon servisi arayüzü.
    /// TCMB döviz kurları ve TÜİK enflasyon verilerini çeker, depolar ve analiz eder.
    /// </summary>
    public interface IMacroEconomyIntegrationService
    {
        /// <summary>
        /// Belirtilen döviz cinsi için güncel kuru getirir.
        /// Önce veritabanını kontrol eder, güncel değilse TCMB'den çeker.
        /// </summary>
        /// <param name="currency">Döviz cinsi (USD, EUR)</param>
        /// <returns>Güncel döviz kuru bilgisi</returns>
        Task<ExchangeRate> GetCurrentExchangeRateAsync(CurrencyType currency);

        /// <summary>
        /// Belirtilen tarih aralığında döviz kuru geçmişini getirir.
        /// Trend analizi ve hareketli ortalama hesaplamaları için kullanılır.
        /// </summary>
        /// <param name="currency">Döviz cinsi</param>
        /// <param name="from">Başlangıç tarihi</param>
        /// <param name="to">Bitiş tarihi</param>
        /// <returns>Tarih aralığındaki döviz kuru listesi</returns>
        Task<List<ExchangeRate>> GetExchangeRateHistoryAsync(CurrencyType currency, DateTime from, DateTime to);

        /// <summary>
        /// En güncel enflasyon verisini getirir (TÜİK TÜFE).
        /// Satın alma gücü ve reel değer hesaplamalarında kullanılır.
        /// </summary>
        /// <returns>Son enflasyon verisi</returns>
        Task<InflationData> GetLatestInflationDataAsync();

        /// <summary>
        /// Belirtilen ay sayısı kadar geriye dönük enflasyon geçmişini getirir.
        /// Enflasyon trendi analizi için kullanılır.
        /// </summary>
        /// <param name="months">Geriye dönük ay sayısı</param>
        /// <returns>Enflasyon veri listesi</returns>
        Task<List<InflationData>> GetInflationHistoryAsync(int months);

        /// <summary>
        /// Günlük döviz kurlarını TCMB'den çekip veritabanına kaydeder.
        /// Zamanlayıcı (scheduler) tarafından günde bir kez çağrılır.
        /// </summary>
        Task SyncDailyRatesAsync();

        /// <summary>
        /// Enflasyon verilerini TÜİK'ten çekip veritabanına kaydeder.
        /// Aylık olarak çalıştırılması önerilir.
        /// </summary>
        Task SyncInflationDataAsync();
    }
}
