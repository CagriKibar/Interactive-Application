using FinancialERP.Entity.Models;

namespace FinancialERP.Entity.DTOs
{
    /// <summary>
    /// Yönetici dashboard DTO'su.
    /// Tüm finansal verilerin kapsamlı özetini sunar.
    /// </summary>
    public class ExecutiveDashboardDto
    {
        // --- Satış bilgileri ---

        /// <summary>Günlük ciro (TRY)</summary>
        public decimal DailyRevenue { get; set; }

        /// <summary>Aylık ciro (TRY)</summary>
        public decimal MonthlyRevenue { get; set; }

        /// <summary>En çok satan ürünler</summary>
        public List<string> TopSellingProducts { get; set; } = new();

        // --- Alt analizler ---

        /// <summary>Nakit akışı sağlık durumu</summary>
        public CashFlowHealthDto CashFlowHealth { get; set; } = new();

        /// <summary>Kar marjı analizleri</summary>
        public List<ProfitMarginAnalysisDto> ProfitMarginAnalyses { get; set; } = new();

        /// <summary>Stok optimizasyon önerileri</summary>
        public List<StockOptimizationDto> StockOptimizations { get; set; } = new();

        /// <summary>Makroekonomik özet</summary>
        public MacroEconomicSummaryDto MacroEconomicSummary { get; set; } = new();

        // --- Bildirimler ---

        /// <summary>Aktif bildirimler</summary>
        public List<Notification> ActiveNotifications { get; set; } = new();
    }
}
