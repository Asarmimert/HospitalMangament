using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HospitalManagement.Entity.Entities
{
    public class Muayene
    {
        [Key]
        public int Id { get; set; }

        public int RandevuId { get; set; }

        public Randevu Randevu { get; set; } = null!;

        [Required]
        public string HastaSikayeti { get; set; } = string.Empty;

        [Required]
        public string DoktorDegerlendirmesi {  get; set; } = string.Empty;


        public string? DoktorNotlari { get; set; }

        public DateTime MuayeneTarihi { get; set; }


        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;


        public DateTime? GuncellenmeTarihi { get; set; }







    }
}
