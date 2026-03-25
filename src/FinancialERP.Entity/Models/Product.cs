using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialERP.Entity.Models
{
    /// <summary>
    /// Barsoft ürün bilgisi: isim, maliyet (TRY/USD), satış fiyatı, stok miktarı
    /// </summary>
    public class Product : BaseEntity
    {
        /// <summary>Ürün adı</summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Ürün kodu / barkod</summary>
        [StringLength(50)]
        public string? ProductCode { get; set; }

        /// <summary>Kategori</summary>
        [StringLength(100)]
        public string? Category { get; set; }

        /// <summary>Maliyet fiyatı (TRY)</summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal CostPriceInTRY { get; set; }

        /// <summary>Maliyet fiyatı (USD) - dövizle alınan ürünler için</summary>
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal? CostPriceInUSD { get; set; }

        /// <summary>Satış fiyatı (TRY)</summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal SalePrice { get; set; }

        /// <summary>Güncel stok miktarı</summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        /// <summary>Minimum stok seviyesi (uyarı eşiği)</summary>
        [Range(0, int.MaxValue)]
        public int MinimumStockLevel { get; set; }

        /// <summary>Kar marjı (%)</summary>
        [NotMapped]
        public decimal ProfitMargin => SalePrice > 0
            ? ((SalePrice - CostPriceInTRY) / SalePrice) * 100
            : 0;

        /// <summary>Ürün aktif mi?</summary>
        [Required]
        public bool IsActive { get; set; } = true;
    }
}
