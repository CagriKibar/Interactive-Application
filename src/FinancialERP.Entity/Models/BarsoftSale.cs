namespace FinancialERP.Entity.Models
{
    public class BarsoftSale
    {
        public long FisNo { get; set; }
        public DateTime SaleDate { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CostPrice { get; set; }
        public decimal ProfitAmount => TotalAmount - (CostPrice * Quantity);
        public decimal ProfitMargin => TotalAmount > 0 ? (ProfitAmount / TotalAmount) * 100 : 0;
    }
}
