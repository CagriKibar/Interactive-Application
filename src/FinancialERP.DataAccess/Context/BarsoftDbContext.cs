using Microsoft.EntityFrameworkCore;

namespace FinancialERP.DataAccess.Context
{
    /// <summary>
    /// Read-only connection to Barsoft ERP MSSQL database.
    /// Only SELECT operations are permitted.
    /// </summary>
    public class BarsoftDbContext : DbContext
    {
        public BarsoftDbContext(DbContextOptions<BarsoftDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Barsoft tables are mapped as keyless views for read-only access
        }
    }
}
