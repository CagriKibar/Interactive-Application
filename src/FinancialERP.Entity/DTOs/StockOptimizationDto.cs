namespace FinancialERP.Entity.DTOs
{
    /// <summary>
    /// Stok optimizasyonu DTO'su.
    /// Döviz kuru trendine göre stok yönetimi önerileri sunar.
    /// </summary>
    public class StockOptimizationDto
    {
        /// <summary>Ürün adı</summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>Mevcut stok miktarı</summary>
        public int CurrentStock { get; set; }

        /// <summary>Aylık ortalama satış adedi</summary>
        public decimal MonthlySalesAverage { get; set; }

        /// <summary>Kalan stok gün sayısı</summary>
        public decimal DaysOfStockLeft { get; set; }

        /// <summary>Döviz kuru trendi (Upward / Downward / Stable)</summary>
        public string ExchangeRateTrend { get; set; } = string.Empty;

        /// <summary>Önerilen aksiyon</summary>
        public string RecommendedAction { get; set; } = string.Empty;

        /// <summary>Tahmini tasarruf tutarı (TRY)</summary>
        public decimal EstimatedSaving { get; set; }

        /// <summary>AI uyarı mesajı</summary>
        public string AlertMessage { get; set; } = string.Empty;
    }
}
