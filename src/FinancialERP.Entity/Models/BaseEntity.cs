using System.ComponentModel.DataAnnotations;

namespace FinancialERP.Entity.Models
{
    /// <summary>
    /// Tüm entity'ler için temel sınıf.
    /// Ortak alanları (Id, oluşturulma/güncellenme tarihi, soft delete) barındırır.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>Birincil anahtar</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>Kayıt oluşturulma tarihi</summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Son güncellenme tarihi</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Soft delete durumu</summary>
        [Required]
        public bool IsDeleted { get; set; } = false;
    }
}
