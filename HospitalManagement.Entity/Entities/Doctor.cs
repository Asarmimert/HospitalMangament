using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Entity.Entities
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        public int KullaniciHesabiId { get; set; }

        public KullaniciHesabi KullaniciHesabi { get; set; } = null!;

        public int DepartmentId { get; set; }

        public Department Department { get; set; } = null!;

        [Required]
        [MaxLength(25)]
        public string DoktorAd { get; set; } = string.Empty;

        [Required]
        [MaxLength(40)]
        public string DoktorSoyad { get; set; } = string.Empty;

        [MaxLength(11)]
        public string? TelefonNumarasi { get; set; }

        [MaxLength(35)]
        public string? UzmanlikAlani { get; set; }

        public bool AktifMi { get; set; } = true;

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;

        public DateTime? GuncellenmeTarihi { get; set; }
    }
}