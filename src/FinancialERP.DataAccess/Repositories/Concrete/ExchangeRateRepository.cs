using FinancialERP.DataAccess.Context;
using FinancialERP.DataAccess.Repositories.Abstract;
using FinancialERP.Entity.Enums;
using FinancialERP.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancialERP.DataAccess.Repositories.Concrete
{
    public class ExchangeRateRepository : GenericRepository<ExchangeRate>, IExchangeRateRepository
    {
        public ExchangeRateRepository(FinancialDbContext context) : base(context) { }

        public async Task<ExchangeRate?> GetLatestRateAsync(CurrencyType currency)
        {
            return await _dbSet
                .Where(r => r.CurrencyType == currency)
                .OrderByDescending(r => r.RateDate)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ExchangeRate>> GetRateHistoryAsync(CurrencyType currency, int days)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            return await _dbSet
                .Where(r => r.CurrencyType == currency && r.RateDate >= startDate)
                .OrderByDescending(r => r.RateDate)
                .ToListAsync();
        }

        public async Task<ExchangeRate?> GetRateByDateAsync(CurrencyType currency, DateTime date)
        {
            return await _dbSet
                .Where(r => r.CurrencyType == currency && r.RateDate.Date == date.Date)
                .FirstOrDefaultAsync();
        }
    }
}
