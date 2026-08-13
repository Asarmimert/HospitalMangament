using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HospitalManagement.Entity.Entities
{
    public class Hasta
    {
        [Key]
        public int Id { get; set; }


        public int KullaniciHesabiId { get; set; }

        public KullaniciHesabi KullaniciHesabi { get; set; } = null!;

        [Required]
        [MaxLength(25)]
        public string Ad { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Soyad { get; set; } = string.Empty;



        [Required]
        [MaxLength(11)]
        public string KimlikNumarasi { get; set; } = string.Empty;

        public DateOnly DogumTarihi {  get; set; }

        [MaxLength(11)]
        [Required]
        public string TelefonNumarasi { get; set; } = null!;

        [MaxLength(300)]
        public string? Adres {  get; set; }


        public bool AktifMi { get; set; } = true;

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
        public DateTime? GuncellenmeTarihi { get; set; }
    }
}
