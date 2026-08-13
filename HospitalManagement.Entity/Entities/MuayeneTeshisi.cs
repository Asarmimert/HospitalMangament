using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HospitalManagement.Entity.Entities
{
    public class MuayeneTeshisi : IEntity
    {

        [Key]
        public int Id { get; set; }

        public int MuayeneId { get; set; }

        public Muayene Muayene { get; set; } = null!;


        public  int TeshisId {  get; set; }


        public Teshis Teshis { get; set; } = null!;


        public string? DoktorNotu {  get; set; }






        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;


        public DateTime? GuncellenmeTarihi { get; set; }
    }
}
