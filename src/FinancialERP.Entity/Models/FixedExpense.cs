using FinancialERP.Entity.Enums;

namespace FinancialERP.Entity.Models
{
    public class FixedExpense : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public ExpenseCategory Category { get; set; }
        public decimal Amount { get; set; }
        public CurrencyType Currency { get; set; } = CurrencyType.TRY;
        public int DayOfMonth { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsRecurring { get; set; } = true;
    }
}
