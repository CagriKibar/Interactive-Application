using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinancialERP.Entity.Enums;

namespace FinancialERP.Entity.Models
{
    /// <summary>
    /// Nakit akışı kayıtları: giren/çıkan para hareketleri
    /// </summary>
    public class CashFlowRecord : BaseEntity
    {
        /// <summary>İşlem tarihi</summary>
        [Required]
        public DateTime TransactionDate { get; set; }

        /// <summary>İşlem açıklaması</summary>
        [Required]
        [StringLength(300)]
        public string Description { get; set; } = string.Empty;

        /// <summary>Giriş tutarı (TRY)</summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal InflowAmount { get; set; }

        /// <summary>Çıkış tutarı (TRY)</summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal OutflowAmount { get; set; }

        /// <summary>Net tutar (giriş - çıkış)</summary>
        [NotMapped]
        public decimal NetAmount => InflowAmount - OutflowAmount;

        /// <summary>Para birimi</summary>
        [Required]
        public CurrencyType Currency { get; set; } = CurrencyType.TRY;

        /// <summary>İlişkili banka hesabı ID'si</summary>
        public int? BankAccountId { get; set; }

        /// <summary>İlişkili banka hesabı</summary>
        [ForeignKey(nameof(BankAccountId))]
        public virtual BankAccount? BankAccount { get; set; }

        /// <summary>Kaynak / referans bilgisi</summary>
        [StringLength(200)]
        public string? ReferenceInfo { get; set; }
    }
}
