using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialERP.Entity.Models
{
    /// <summary>
    /// Günlük döviz kuru takibi (USD/TRY, EUR/TRY)
    /// </summary>
    public class ExchangeRate : BaseEntity
    {
        /// <summary>Kur tarihi</summary>
        [Required]
        public DateTime RateDate { get; set; }

        /// <summary>USD/TRY alış kuru</summary>
        [Required]
        [Column(TypeName = "decimal(18,6)")]
        [Range(0, double.MaxValue)]
        public decimal USDRate { get; set; }

        /// <summary>EUR/TRY alış kuru</summary>
        [Required]
        [Column(TypeName = "decimal(18,6)")]
        [Range(0, double.MaxValue)]
        public decimal EURRate { get; set; }

        /// <summary>USD/TRY satış kuru</summary>
        [Column(TypeName = "decimal(18,6)")]
        public decimal? USDSellRate { get; set; }

        /// <summary>EUR/TRY satış kuru</summary>
        [Column(TypeName = "decimal(18,6)")]
        public decimal? EURSellRate { get; set; }

        /// <summary>Veri kaynağı (ör: TCMB, serbest piyasa)</summary>
        [StringLength(50)]
        public string? Source { get; set; }
    }
}
