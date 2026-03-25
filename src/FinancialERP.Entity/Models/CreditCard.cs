using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialERP.Entity.Models
{
    /// <summary>
    /// Kredi kartı takibi: limit, harcama, ekstre ve son ödeme tarihleri, asgari ödeme
    /// </summary>
    public class CreditCard : BaseEntity
    {
        /// <summary>Kart adı / tanımı (ör: "Yapı Kredi Platinum")</summary>
        [Required]
        [StringLength(100)]
        public string CardName { get; set; } = string.Empty;

        /// <summary>Banka adı</summary>
        [Required]
        [StringLength(100)]
        public string BankName { get; set; } = string.Empty;

        /// <summary>Kartın son 4 hanesi</summary>
        [StringLength(4, MinimumLength = 4)]
        public string? LastFourDigits { get; set; }

        /// <summary>Toplam kart limiti (TRY)</summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal CreditLimit { get; set; }

        /// <summary>Güncel harcama tutarı (TRY)</summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal CurrentSpending { get; set; }

        /// <summary>Kullanılabilir limit (TRY)</summary>
        [NotMapped]
        public decimal AvailableLimit => CreditLimit - CurrentSpending;

        /// <summary>Ekstre kesim günü (ayın kaçı)</summary>
        [Required]
        [Range(1, 31)]
        public int StatementDay { get; set; }

        /// <summary>Son ödeme günü (ayın kaçı)</summary>
        [Required]
        [Range(1, 31)]
        public int DueDay { get; set; }

        /// <summary>Asgari ödeme tutarı (TRY)</summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal MinimumPayment { get; set; }

        /// <summary>Son ekstre tarihi</summary>
        public DateTime? LastStatementDate { get; set; }

        /// <summary>Son ödeme tarihi</summary>
        public DateTime? NextDueDate { get; set; }

        /// <summary>Kart aktif mi?</summary>
        [Required]
        public bool IsActive { get; set; } = true;
    }
}
