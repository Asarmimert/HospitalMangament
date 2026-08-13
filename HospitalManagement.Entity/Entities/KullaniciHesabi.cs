using System;
using System.Collections.Generic;
using System.Text;
using HospitalManagement.Entity.Enums;
using System.ComponentModel.DataAnnotations;
namespace HospitalManagement.Entity.Entities
{
    public class KullaniciHesabi
    {



        [Key]
        public int Id {  get; set; }
        [Required]
        [MaxLength(150)]
        public string Eposta { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string ParolaHash { get; set; } = string.Empty;

        public KullaniciRolu Rol {  get; set; }

        public bool AktifMi { get; set; } = true;

        public DateTime OlusturulmaTarihi {  get; set; } = DateTime.UtcNow;


        public DateTime? GuncellenmeTarihi { get; set; }













    }
}
