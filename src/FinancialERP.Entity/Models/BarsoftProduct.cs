namespace FinancialERP.Entity.Models
{
    public class BarsoftProduct
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal SalePrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal CurrentStock { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal KdvRate { get; set; }
    }
}
