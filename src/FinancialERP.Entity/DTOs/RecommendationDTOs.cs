using FinancialERP.Entity.Enums;

namespace FinancialERP.Entity.DTOs
{
    public class FinancialRecommendation
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string DetailedAnalysis { get; set; } = string.Empty;
        public AlertSeverity Severity { get; set; }
        public AlertCategory Category { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> MetaData { get; set; } = new();
        public List<string> ActionItems { get; set; } = new();
        public decimal? ImpactAmount { get; set; }
        public string ImpactPercentage { get; set; } = string.Empty;
    }

    public class ProfitMarginAnalysis
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal CurrentSalePrice { get; set; }
        public decimal CurrentCostPrice { get; set; }
        public decimal CostPriceInUSD { get; set; }
        public decimal PreviousMarginPercent { get; set; }
        public decimal CurrentMarginPercent { get; set; }
        public decimal MarginChangePercent { get; set; }
        public decimal SuggestedNewPrice { get; set; }
        public string CurrencyImpactNote { get; set; } = string.Empty;
    }

    public class CashFlowReport
    {
        public decimal TotalBankBalance { get; set; }
        public decimal TotalBankBalanceInUSD { get; set; }
        public decimal TotalCreditCardDebt { get; set; }
        public decimal UpcomingMonthlyExpenses { get; set; }
        public decimal ProjectedMonthlyRevenue { get; set; }
        public decimal NetCashPosition { get; set; }
        public decimal RealPurchasingPowerTRY { get; set; }
        public CashFlowHealth HealthStatus { get; set; }
        public decimal InflationAdjustedValue { get; set; }
        public List<FinancialRecommendation> Recommendations { get; set; } = new();
    }

    public class StockOptimizationSuggestion
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal CurrentStock { get; set; }
        public decimal AverageDailySales { get; set; }
        public int EstimatedDaysUntilStockout { get; set; }
        public decimal SuggestedOrderQuantity { get; set; }
        public decimal EstimatedOrderCost { get; set; }
        public string Rationale { get; set; } = string.Empty;
    }

    public class DashboardSummary
    {
        public decimal TodayRevenue { get; set; }
        public decimal TodayProfit { get; set; }
        public decimal UsdRate { get; set; }
        public decimal EurRate { get; set; }
        public decimal UsdChangePercent { get; set; }
        public decimal EurChangePercent { get; set; }
        public decimal MonthlyInflationRate { get; set; }
        public decimal AnnualInflationRate { get; set; }
        public CashFlowReport CashFlow { get; set; } = new();
        public List<ProfitMarginAnalysis> ErodedMargins { get; set; } = new();
        public List<StockOptimizationSuggestion> StockAlerts { get; set; } = new();
        public List<FinancialRecommendation> Notifications { get; set; } = new();
    }

    public class MacroEconomicSnapshot
    {
        public decimal UsdBuyRate { get; set; }
        public decimal UsdSellRate { get; set; }
        public decimal EurBuyRate { get; set; }
        public decimal EurSellRate { get; set; }
        public decimal UsdDailyChange { get; set; }
        public decimal EurDailyChange { get; set; }
        public decimal MonthlyInflation { get; set; }
        public decimal AnnualInflation { get; set; }
        public DateTime LastUpdated { get; set; }
        public string TrendDirection { get; set; } = string.Empty;
    }
}
