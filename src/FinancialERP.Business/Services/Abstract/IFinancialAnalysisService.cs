using FinancialERP.Entity.DTOs;
using FinancialERP.Entity.Models;

namespace FinancialERP.Business.Services.Abstract
{
    /// <summary>
    /// Finansal analiz ve öneri motoru arayüzü.
    /// Döviz/enflasyon verileri ile ERP stok verisini harmanlayarak
    /// kâr marjı, stok optimizasyonu ve nakit akışı analizleri üretir.
    /// </summary>
    public interface IFinancialAnalysisService
    {
        /// <summary>
        /// Barsoft'tan çekilen ürün satış fiyatları ile güncel kur/enflasyon verilerini
        /// karşılaştırarak kâr marjı erimesini tespit eder.
        /// </summary>
        Task<List<ProfitMarginAnalysis>> AnalyzeProfitMarginsAsync();

        /// <summary>
        /// Nakit pozisyonu, döviz trendi ve en çok satan ürünleri analiz ederek
        /// stok artırma/azaltma önerileri üretir.
        /// </summary>
        Task<List<StockOptimizationSuggestion>> AnalyzeStockOptimizationAsync();

        /// <summary>
        /// İşletmenin nakit akışı sağlığını ölçer.
        /// Banka bakiyeleri, kredi kartı borçları, sabit giderler ve gelirleri
        /// enflasyon ve döviz karşısında değerlendirerek skor üretir.
        /// </summary>
        Task<CashFlowReport> AnalyzeCashFlowHealthAsync();

        /// <summary>
        /// Makroekonomik özet (döviz, enflasyon, trend) verisini derler.
        /// </summary>
        Task<MacroEconomicSnapshot> GetMacroEconomicSummaryAsync();

        /// <summary>
        /// Tüm analizleri bir araya getirerek patron için yönetici özet panosu üretir.
        /// </summary>
        Task<DashboardSummary> GenerateExecutiveDashboardAsync();

        /// <summary>
        /// Tüm analiz sonuçlarından akıllı bildirimler/uyarılar üretir.
        /// </summary>
        Task<List<FinancialRecommendation>> GenerateSmartNotificationsAsync();
    }
}
