namespace FinancialERP.Entity.Enums
{
    public enum ExpenseCategory
    {
        Salary = 1,
        CreditCard = 2,
        Electricity = 3,
        Water = 4,
        Internet = 5,
        Fuel = 6,
        Rent = 7,
        Insurance = 8,
        Tax = 9,
        Other = 10
    }

    public enum CurrencyType
    {
        TRY = 1,
        USD = 2,
        EUR = 3
    }

    public enum AlertSeverity
    {
        Info = 0,
        Warning = 1,
        Critical = 2,
        Opportunity = 3
    }

    public enum AlertCategory
    {
        ProfitMargin = 1,
        StockOptimization = 2,
        CashFlow = 3,
        CurrencyRisk = 4,
        InflationImpact = 5,
        FinancingAdvice = 6
    }

    public enum CashFlowHealth
    {
        Healthy = 1,
        Moderate = 2,
        Warning = 3,
        Critical = 4
    }
}
