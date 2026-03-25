using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialERP.Entity.Models
{
    /// <summary>
    /// Barsoft günlük satış özeti
    /// </summary>
    public class DailySales : BaseEntity
    {
        /// <summary>Satış tarihi</summary>
        [Required]
        public DateTime SalesDate { get; set; }

        /// <summary>Toplam satış tutarı (TRY)</summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal TotalRevenue { get; set; }

        /// <summary>Toplam satış adedi</summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int TotalTransactions { get; set; }

        /// <summary>Nakit satış tutarı (TRY)</summary>
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal CashRevenue { get; set; }

        /// <summary>Kredi kartı ile satış tutarı (TRY)</summary>
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal CreditCardRevenue { get; set; }

        /// <summary>Toplam maliyet (TRY)</summary>
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal TotalCost { get; set; }

        /// <summary>Brüt kar (TRY)</summary>
        [NotMapped]
        public decimal GrossProfit => TotalRevenue - TotalCost;

        /// <summary>Brüt kar marjı (%)</summary>
        [NotMapped]
        public decimal GrossProfitMargin => TotalRevenue > 0
            ? (GrossProfit / TotalRevenue) * 100
            : 0;

        /// <summary>Ek notlar</summary>
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
