using FinancialERP.Entity.Models;

namespace FinancialERP.DataAccess.BarsoftIntegration
{
    public interface IBarsoftDataService
    {
        Task<IEnumerable<BarsoftSale>> GetDailySalesAsync(DateTime date);
        Task<IEnumerable<BarsoftSale>> GetSalesInRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<BarsoftProduct>> GetAllProductsAsync();
        Task<BarsoftProduct?> GetProductByCodeAsync(string productCode);
        Task<IEnumerable<BarsoftProduct>> GetTopSellingProductsAsync(int days, int topN = 10);
        Task<decimal> GetDailyRevenueAsync(DateTime date);
        Task<decimal> GetCashRegisterBalanceAsync();
    }
}
