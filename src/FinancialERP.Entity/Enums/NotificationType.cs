namespace FinancialERP.Entity.Enums
{
    /// <summary>
    /// AI bildirim türleri
    /// </summary>
    public enum NotificationType
    {
        /// <summary>Kar marjı uyarısı</summary>
        ProfitMarginAlert = 0,

        /// <summary>Stok optimizasyonu önerisi</summary>
        StockOptimization = 1,

        /// <summary>Nakit akışı uyarısı</summary>
        CashFlowWarning = 2,

        /// <summary>Döviz kuru uyarısı</summary>
        ExchangeRateAlert = 3,

        /// <summary>Enflasyon etkisi analizi</summary>
        InflationImpact = 4,

        /// <summary>Finansman önerisi</summary>
        FinancingRecommendation = 5
    }
}
