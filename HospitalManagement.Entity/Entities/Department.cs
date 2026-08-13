using System.ComponentModel.DataAnnotations;
namespace HospitalManagement.Entity.Entities
{
    public class Department 
    {
        [Key]
        public int DepartmentId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;


        [MaxLength(200)]
        public string? Description { get; set; }

        public bool AktifMi { get; set; } = true;

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;


        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();




    }
}
