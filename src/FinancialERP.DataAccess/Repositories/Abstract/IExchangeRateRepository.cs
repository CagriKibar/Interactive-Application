using FinancialERP.Entity.Enums;
using FinancialERP.Entity.Models;

namespace FinancialERP.DataAccess.Repositories.Abstract
{
    public interface IExchangeRateRepository : IGenericRepository<ExchangeRate>
    {
        Task<ExchangeRate?> GetLatestRateAsync(CurrencyType currency);
        Task<IEnumerable<ExchangeRate>> GetRateHistoryAsync(CurrencyType currency, int days);
        Task<ExchangeRate?> GetRateByDateAsync(CurrencyType currency, DateTime date);
    }
}
