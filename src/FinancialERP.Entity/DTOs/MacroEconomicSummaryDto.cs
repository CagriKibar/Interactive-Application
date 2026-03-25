namespace FinancialERP.Entity.DTOs
{
    /// <summary>
    /// Makroekonomik özet DTO'su.
    /// Döviz kuru, enflasyon ve satın alma gücü bilgilerini içerir.
    /// </summary>
    public class MacroEconomicSummaryDto
    {
        /// <summary>Güncel USD/TRY kuru</summary>
        public decimal CurrentUSDRate { get; set; }

        /// <summary>Güncel EUR/TRY kuru</summary>
        public decimal CurrentEURRate { get; set; }

        /// <summary>USD kuru 30 günlük değişim yüzdesi (%)</summary>
        public decimal USDChangePercent30Days { get; set; }

        /// <summary>Aylık enflasyon oranı (%)</summary>
        public decimal MonthlyInflationRate { get; set; }

        /// <summary>Yıllık enflasyon oranı (%)</summary>
        public decimal YearlyInflationRate { get; set; }

        /// <summary>Satın alma gücü endeksi</summary>
        public decimal PurchasingPowerIndex { get; set; }

        /// <summary>Makroekonomik uyarılar listesi</summary>
        public List<string> Alerts { get; set; } = new();
    }
}
