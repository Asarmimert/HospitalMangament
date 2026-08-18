using System;
using System.ComponentModel.DataAnnotations;
using HospitalManagement.Entity.Enums;

namespace HospitalManagement.Entity.Entities
{
    public class Randevu
    {
        [Key]
        public int Id { get; set; }

        public int DoktorId { get; set; }

        public Doctor Doktor { get; set; } = null!;

        public int HastaId { get; set; }

        public Hasta Hasta { get; set; } = null!;

        public int? OlusturanSekreterId { get; set; }

        public Sekreter? OlusturanSekreter { get; set; }

        public DateTime BaslangicZamani { get; set; }

        public DateTime BitisZamani { get; set; }

        public RandevuDurumu Durum { get; set; }

        [MaxLength(300)]
        public string? IptalNedeni { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }
            = DateTime.UtcNow;

        public DateTime? GuncellenmeTarihi { get; set; }
    }
}