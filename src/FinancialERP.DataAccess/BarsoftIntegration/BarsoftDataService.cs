using FinancialERP.Entity.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace FinancialERP.DataAccess.BarsoftIntegration
{
    public class BarsoftDataService : IBarsoftDataService
    {
        private readonly string _connectionString;

        public BarsoftDataService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BarsoftReadOnly")
                ?? throw new InvalidOperationException("BarsoftReadOnly connection string not found.");
        }

        private SqlConnection CreateConnection()
        {
            var connection = new SqlConnection(_connectionString);
            return connection;
        }

        public async Task<IEnumerable<BarsoftSale>> GetDailySalesAsync(DateTime date)
        {
            var sales = new List<BarsoftSale>();
            using var connection = CreateConnection();
            await connection.OpenAsync();

            // Barsoft STOK_HAREKETLERI tablosundan günlük satış çekme
            var query = @"
                SELECT
                    sh.FISNO, sh.TARIH, sh.STOKKODU, s.STOKADI,
                    sh.MIKTAR, sh.BIRIMFIYAT, sh.TUTAR, s.MALIYETFIYATI
                FROM STOK_HAREKETLERI sh WITH (NOLOCK)
                INNER JOIN STOKLAR s WITH (NOLOCK) ON sh.STOKKODU = s.STOKKODU
                WHERE CAST(sh.TARIH AS DATE) = @SaleDate
                    AND sh.HAREKETTURU = 1 -- Satış
                ORDER BY sh.TARIH DESC";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SaleDate", date.Date);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                sales.Add(new BarsoftSale
                {
                    FisNo = reader.GetInt64(0),
                    SaleDate = reader.GetDateTime(1),
                    ProductCode = reader.GetString(2),
                    ProductName = reader.GetString(3),
                    Quantity = reader.GetDecimal(4),
                    UnitPrice = reader.GetDecimal(5),
                    TotalAmount = reader.GetDecimal(6),
                    CostPrice = reader.GetDecimal(7)
                });
            }

            return sales;
        }

        public async Task<IEnumerable<BarsoftSale>> GetSalesInRangeAsync(DateTime startDate, DateTime endDate)
        {
            var sales = new List<BarsoftSale>();
            using var connection = CreateConnection();
            await connection.OpenAsync();

            var query = @"
                SELECT
                    sh.FISNO, sh.TARIH, sh.STOKKODU, s.STOKADI,
                    sh.MIKTAR, sh.BIRIMFIYAT, sh.TUTAR, s.MALIYETFIYATI
                FROM STOK_HAREKETLERI sh WITH (NOLOCK)
                INNER JOIN STOKLAR s WITH (NOLOCK) ON sh.STOKKODU = s.STOKKODU
                WHERE sh.TARIH BETWEEN @StartDate AND @EndDate
                    AND sh.HAREKETTURU = 1
                ORDER BY sh.TARIH DESC";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@StartDate", startDate.Date);
            command.Parameters.AddWithValue("@EndDate", endDate.Date);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                sales.Add(new BarsoftSale
                {
                    FisNo = reader.GetInt64(0),
                    SaleDate = reader.GetDateTime(1),
                    ProductCode = reader.GetString(2),
                    ProductName = reader.GetString(3),
                    Quantity = reader.GetDecimal(4),
                    UnitPrice = reader.GetDecimal(5),
                    TotalAmount = reader.GetDecimal(6),
                    CostPrice = reader.GetDecimal(7)
                });
            }

            return sales;
        }

        public async Task<IEnumerable<BarsoftProduct>> GetAllProductsAsync()
        {
            var products = new List<BarsoftProduct>();
            using var connection = CreateConnection();
            await connection.OpenAsync();

            var query = @"
                SELECT STOKKODU, STOKADI, SATISFIYATI1, MALIYETFIYATI,
                       BAKIYE, BIRIM, GRUBU, KDVORANI
                FROM STOKLAR WITH (NOLOCK)
                WHERE AKTIF = 1
                ORDER BY STOKADI";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new BarsoftProduct
                {
                    ProductCode = reader.GetString(0),
                    ProductName = reader.GetString(1),
                    SalePrice = reader.GetDecimal(2),
                    CostPrice = reader.GetDecimal(3),
                    CurrentStock = reader.GetDecimal(4),
                    Unit = reader.IsDBNull(5) ? "Adet" : reader.GetString(5),
                    Category = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    KdvRate = reader.GetDecimal(7)
                });
            }

            return products;
        }

        public async Task<BarsoftProduct?> GetProductByCodeAsync(string productCode)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            var query = @"
                SELECT STOKKODU, STOKADI, SATISFIYATI1, MALIYETFIYATI,
                       BAKIYE, BIRIM, GRUBU, KDVORANI
                FROM STOKLAR WITH (NOLOCK)
                WHERE STOKKODU = @ProductCode";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductCode", productCode);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new BarsoftProduct
                {
                    ProductCode = reader.GetString(0),
                    ProductName = reader.GetString(1),
                    SalePrice = reader.GetDecimal(2),
                    CostPrice = reader.GetDecimal(3),
                    CurrentStock = reader.GetDecimal(4),
                    Unit = reader.IsDBNull(5) ? "Adet" : reader.GetString(5),
                    Category = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    KdvRate = reader.GetDecimal(7)
                };
            }

            return null;
        }

        public async Task<IEnumerable<BarsoftProduct>> GetTopSellingProductsAsync(int days, int topN = 10)
        {
            var products = new List<BarsoftProduct>();
            using var connection = CreateConnection();
            await connection.OpenAsync();

            var query = @"
                SELECT TOP(@TopN)
                    s.STOKKODU, s.STOKADI, s.SATISFIYATI1, s.MALIYETFIYATI,
                    s.BAKIYE, s.BIRIM, s.GRUBU, s.KDVORANI
                FROM STOKLAR s WITH (NOLOCK)
                INNER JOIN (
                    SELECT STOKKODU, SUM(MIKTAR) AS ToplamSatis
                    FROM STOK_HAREKETLERI WITH (NOLOCK)
                    WHERE HAREKETTURU = 1
                        AND TARIH >= DATEADD(DAY, -@Days, GETDATE())
                    GROUP BY STOKKODU
                ) sh ON s.STOKKODU = sh.STOKKODU
                ORDER BY sh.ToplamSatis DESC";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TopN", topN);
            command.Parameters.AddWithValue("@Days", days);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new BarsoftProduct
                {
                    ProductCode = reader.GetString(0),
                    ProductName = reader.GetString(1),
                    SalePrice = reader.GetDecimal(2),
                    CostPrice = reader.GetDecimal(3),
                    CurrentStock = reader.GetDecimal(4),
                    Unit = reader.IsDBNull(5) ? "Adet" : reader.GetString(5),
                    Category = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    KdvRate = reader.GetDecimal(7)
                });
            }

            return products;
        }

        public async Task<decimal> GetDailyRevenueAsync(DateTime date)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            var query = @"
                SELECT ISNULL(SUM(TUTAR), 0)
                FROM STOK_HAREKETLERI WITH (NOLOCK)
                WHERE CAST(TARIH AS DATE) = @SaleDate AND HAREKETTURU = 1";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SaleDate", date.Date);

            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToDecimal(result) : 0;
        }

        public async Task<decimal> GetCashRegisterBalanceAsync()
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            var query = @"
                SELECT ISNULL(SUM(CASE WHEN HAREKETTURU = 0 THEN TUTAR ELSE -TUTAR END), 0)
                FROM KASA_HAREKETLERI WITH (NOLOCK)
                WHERE KASANO = 1";

            using var command = new SqlCommand(query, connection);
            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToDecimal(result) : 0;
        }
    }
}
