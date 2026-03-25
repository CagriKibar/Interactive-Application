using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinancialERP.Entity.Enums;

namespace FinancialERP.Entity.Models
{
    /// <summary>
    /// Sabit gider takibi: personel maaşları, kredi kartı ödemeleri, faturalar vb.
    /// </summary>
    public class Expense : BaseEntity
    {
        /// <summary>Gider açıklaması</summary>
        [Required]
        [StringLength(250)]
        public string Description { get; set; } = string.Empty;

        /// <summary>Gider kategorisi</summary>
        [Required]
        public ExpenseCategory Category { get; set; }

        /// <summary>Gider tutarı (TRY)</summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Tutar sıfırdan büyük olmalıdır.")]
        public decimal Amount { get; set; }

        /// <summary>Para birimi</summary>
        [Required]
        public CurrencyType Currency { get; set; } = CurrencyType.TRY;

        /// <summary>Giderin ait olduğu tarih</summary>
        [Required]
        public DateTime ExpenseDate { get; set; }

        /// <summary>Ödeme yapıldı mı?</summary>
        [Required]
        public bool IsPaid { get; set; } = false;

        /// <summary>Ödeme tarihi</summary>
        public DateTime? PaidDate { get; set; }

        /// <summary>Tekrarlayan gider mi? (aylık sabit giderler için)</summary>
        [Required]
        public bool IsRecurring { get; set; } = false;

        /// <summary>Ek notlar</summary>
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
