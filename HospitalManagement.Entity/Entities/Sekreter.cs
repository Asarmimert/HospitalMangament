using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace HospitalManagement.Entity.Entities
{
    public class Sekreter
    {

        [Key]
        public int Id {  get; set; }

        
        public int KullaniciHesabiId {  get; set; }

        public KullaniciHesabi KullaniciHesabi { get; set; } = null!;

        [Required]
        [MaxLength(25)]
        public string Ad { get; set; } = string.Empty;


        [Required]
        [MaxLength(35)]
        public string Soyad { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string TelefonNumarasi { get; set; } = string.Empty;


        public bool AktifMi { get; set; } = true;

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;


        public DateTime? GuncellenmeTarihi { get; set; }







    }
}
