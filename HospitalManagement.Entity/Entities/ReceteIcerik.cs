using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HospitalManagement.Entity.Entities
{
    public class ReceteIcerik
    {

        [Key]
        public int Id { get; set; }

        public int ReceteId { get; set; }

        public Recete Recete { get; set; } = null!;

        public int IlacId { get; set; }

        public Ilac Ilac { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string KullanimTalimatlari { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string KullanimSuresi { get; set; } = string.Empty;

        public int Miktar { get; set; }

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;

        public DateTime? GuncellenmeTarihi { get; set; }













    }
}
