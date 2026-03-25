using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinancialERP.Entity.Enums;

namespace FinancialERP.Entity.Models
{
    /// <summary>
    /// Banka hesabı nakit ve faiz takibi
    /// </summary>
    public class BankAccount : BaseEntity
    {
        /// <summary>Banka adı</summary>
        [Required]
        [StringLength(100)]
        public string BankName { get; set; } = string.Empty;

        /// <summary>Hesap adı / tanımı</summary>
        [Required]
        [StringLength(150)]
        public string AccountName { get; set; } = string.Empty;

        /// <summary>IBAN numarası</summary>
        [StringLength(34)]
        public string? IBAN { get; set; }

        /// <summary>Hesap para birimi</summary>
        [Required]
        public CurrencyType Currency { get; set; } = CurrencyType.TRY;

        /// <summary>Güncel bakiye</summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        /// <summary>Yıllık faiz oranı (%)</summary>
        [Column(TypeName = "decimal(8,4)")]
        [Range(0, 100)]
        public decimal InterestRate { get; set; }

        /// <summary>Son faiz işlem tarihi</summary>
        public DateTime? LastInterestDate { get; set; }

        /// <summary>Birikmiş faiz tutarı</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal AccruedInterest { get; set; }

        /// <summary>Hesap aktif mi?</summary>
        [Required]
        public bool IsActive { get; set; } = true;
    }
}
