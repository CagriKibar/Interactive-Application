using FinancialERP.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancialERP.DataAccess.Context
{
    public class FinancialDbContext : DbContext
    {
        public FinancialDbContext(DbContextOptions<FinancialDbContext> options) : base(options) { }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<InflationData> InflationData { get; set; }
        public DbSet<FixedExpense> FixedExpenses { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.HasIndex(e => new { e.CurrencyType, e.RateDate }).IsUnique();
                entity.Property(e => e.BuyRate).HasPrecision(18, 6);
                entity.Property(e => e.SellRate).HasPrecision(18, 6);
            });

            modelBuilder.Entity<InflationData>(entity =>
            {
                entity.HasIndex(e => new { e.Year, e.Month }).IsUnique();
                entity.Property(e => e.MonthlyRate).HasPrecision(10, 4);
                entity.Property(e => e.AnnualRate).HasPrecision(10, 4);
                entity.Property(e => e.ProducerPriceIndexMonthly).HasPrecision(10, 4);
                entity.Property(e => e.ProducerPriceIndexAnnual).HasPrecision(10, 4);
            });

            modelBuilder.Entity<FixedExpense>(entity =>
            {
                entity.Property(e => e.Amount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<CreditCard>(entity =>
            {
                entity.Property(e => e.CreditLimit).HasPrecision(18, 2);
                entity.Property(e => e.CurrentSpending).HasPrecision(18, 2);
                entity.Property(e => e.MinimumPayment).HasPrecision(18, 2);
                entity.Property(e => e.InterestRate).HasPrecision(10, 4);
            });

            modelBuilder.Entity<BankAccount>(entity =>
            {
                entity.Property(e => e.Balance).HasPrecision(18, 2);
                entity.Property(e => e.InterestRate).HasPrecision(10, 4);
            });
        }
    }
}
