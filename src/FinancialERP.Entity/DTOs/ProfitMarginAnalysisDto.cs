namespace FinancialERP.Entity.DTOs
{
    /// <summary>
    /// Kar marjı analizi sonuç DTO'su.
    /// Döviz kuru değişimlerinin ürün karlılığına etkisini gösterir.
    /// </summary>
    public class ProfitMarginAnalysisDto
    {
        /// <summary>Ürün adı</summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>Güncel satış fiyatı (TRY)</summary>
        public decimal CurrentSalePrice { get; set; }

        /// <summary>Maliyet fiyatı (TRY)</summary>
        public decimal CostPriceInTRY { get; set; }

        /// <summary>Maliyet fiyatı (USD)</summary>
        public decimal CostPriceInUSD { get; set; }

        /// <summary>Güncel döviz kuru (USD/TRY)</summary>
        public decimal CurrentExchangeRate { get; set; }

        /// <summary>Önceki dönem döviz kuru (USD/TRY)</summary>
        public decimal PreviousExchangeRate { get; set; }

        /// <summary>Marj erozyon yüzdesi - kur değişiminden kaynaklanan kayıp (%)</summary>
        public decimal MarginErosionPercent { get; set; }

        /// <summary>Önerilen yeni satış fiyatı (TRY)</summary>
        public decimal RecommendedNewPrice { get; set; }

        /// <summary>AI uyarı mesajı</summary>
        public string AlertMessage { get; set; } = string.Empty;
    }
}
