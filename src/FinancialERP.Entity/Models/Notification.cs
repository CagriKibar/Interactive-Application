using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinancialERP.Entity.Enums;

namespace FinancialERP.Entity.Models
{
    /// <summary>
    /// AI öneri bildirimleri: önem seviyesine göre sınıflandırılmış akıllı bildirimler
    /// </summary>
    public class Notification : BaseEntity
    {
        /// <summary>Bildirim başlığı</summary>
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>Bildirim detay mesajı</summary>
        [Required]
        [StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        /// <summary>Bildirim türü</summary>
        [Required]
        public NotificationType Type { get; set; }

        /// <summary>Önem seviyesi</summary>
        [Required]
        public NotificationSeverity Severity { get; set; }

        /// <summary>Okundu mu?</summary>
        [Required]
        public bool IsRead { get; set; } = false;

        /// <summary>Okunma tarihi</summary>
        public DateTime? ReadAt { get; set; }

        /// <summary>Bildirim geçerlilik bitiş tarihi</summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>AI tarafından hesaplanan güven skoru (0-100)</summary>
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100)]
        public decimal? ConfidenceScore { get; set; }

        /// <summary>İlişkili veri (JSON formatında ek bilgi)</summary>
        [StringLength(4000)]
        public string? RelatedData { get; set; }
    }
}
