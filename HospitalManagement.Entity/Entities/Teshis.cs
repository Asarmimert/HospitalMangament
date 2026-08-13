using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HospitalManagement.Entity.Entities
{
    public class Teshis
    {
     [Key]
      public int Id { get; set; }
        [MaxLength(100)]
      public string? TeshisKodu { get; set; }
        [Required]
        [MaxLength(250)]
        public string TeshisAdi { get; set; } = string.Empty;

    public string? Aciklama {  get; set; }

        public bool AktifMi { get; set; } = true;

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;


        public DateTime? GuncellenmeTarihi { get; set; }





    }
}
