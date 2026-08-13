using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Entity.Entities
{
    public class Recete
    {
        [Key]
        public int Id { get; set; }

        public int MuayeneId { get; set; }

        public Muayene Muayene { get; set; } = null!;

        public int HastaId { get; set; }

        public Hasta Hasta { get; set; } = null!;

        public int DoktorId { get; set; }

        public Doctor Doktor { get; set; } = null!;

        public DateTime ReceteTarihi { get; set; }

        public string? GenelNotlar { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }
            = DateTime.UtcNow;

        public DateTime? GuncellenmeTarihi { get; set; }
    }
}