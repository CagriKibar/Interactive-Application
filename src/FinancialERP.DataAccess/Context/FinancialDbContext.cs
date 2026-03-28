using FinancialERP.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancialERP.DataAccess.Context
{
    public class FinancialDbContext : DbContext
    {
        public FinancialDbContext(DbContextOptions<FinancialDbContext> options) : base(options) { }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<InflationData> InflationData { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<FixedExpense> FixedExpenses { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<DailySales> DailySales { get; set; }
        public DbSet<CashFlowRecord> CashFlowRecords { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Soft delete global query filter
            modelBuilder.Entity<ExchangeRate>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Expense>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<CreditCard>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<BankAccount>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Product>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Notification>().HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.HasIndex(e => e.RateDate);
                entity.Property(e => e.USDRate).HasPrecision(18, 6);
                entity.Property(e => e.EURRate).HasPrecision(18, 6);
            });

            modelBuilder.Entity<InflationData>(entity =>
            {
                entity.HasIndex(e => new { e.Year, e.Month }).IsUnique();
                entity.Property(e => e.MonthlyRate).HasPrecision(10, 4);
                entity.Property(e => e.YearlyRate).HasPrecision(10, 4);
            });

            modelBuilder.Entity<FixedExpense>(entity =>
            {
                entity.Property(e => e.Amount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.Property(e => e.Amount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<CreditCard>(entity =>
            {
                entity.Property(e => e.CreditLimit).HasPrecision(18, 2);
                entity.Property(e => e.CurrentSpending).HasPrecision(18, 2);
                entity.Property(e => e.MinimumPayment).HasPrecision(18, 2);
            });

            modelBuilder.Entity<BankAccount>(entity =>
            {
                entity.Property(e => e.Balance).HasPrecision(18, 2);
                entity.Property(e => e.InterestRate).HasPrecision(10, 4);
                entity.Property(e => e.AccruedInterest).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.CostPriceInTRY).HasPrecision(18, 2);
                entity.Property(e => e.SalePrice).HasPrecision(18, 2);
            });

            modelBuilder.Entity<DailySales>(entity =>
            {
                entity.HasIndex(e => e.SalesDate).IsUnique();
                entity.Property(e => e.TotalRevenue).HasPrecision(18, 2);
                entity.Property(e => e.TotalCost).HasPrecision(18, 2);
                entity.Property(e => e.CashRevenue).HasPrecision(18, 2);
                entity.Property(e => e.CreditCardRevenue).HasPrecision(18, 2);
            });

            modelBuilder.Entity<CashFlowRecord>(entity =>
            {
                entity.Property(e => e.InflowAmount).HasPrecision(18, 2);
                entity.Property(e => e.OutflowAmount).HasPrecision(18, 2);
            });
        }
    }
}
