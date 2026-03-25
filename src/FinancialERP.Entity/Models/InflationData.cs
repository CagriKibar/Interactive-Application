using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialERP.Entity.Models
{
    /// <summary>
    /// Aylık ve yıllık enflasyon verileri (TÜİK bazlı)
    /// </summary>
    public class InflationData : BaseEntity
    {
        /// <summary>Veri yılı</summary>
        [Required]
        [Range(2000, 2100)]
        public int Year { get; set; }

        /// <summary>Veri ayı (1-12)</summary>
        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        /// <summary>Aylık enflasyon oranı (%)</summary>
        [Required]
        [Column(TypeName = "decimal(8,4)")]
        public decimal MonthlyRate { get; set; }

        /// <summary>Yıllık enflasyon oranı (%)</summary>
        [Required]
        [Column(TypeName = "decimal(8,4)")]
        public decimal YearlyRate { get; set; }

        /// <summary>TÜFE endeks değeri</summary>
        [Column(TypeName = "decimal(12,4)")]
        public decimal? CPIIndex { get; set; }

        /// <summary>Veri kaynağı (ör: TÜİK)</summary>
        [StringLength(50)]
        public string? Source { get; set; }
    }
}
