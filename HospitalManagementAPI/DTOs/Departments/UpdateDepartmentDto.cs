using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Departments
{
    public class UpdateDepartmentDto
    {
        [Required(ErrorMessage = "Bölüm Adınızı Giriniz:")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;



        [MaxLength(50)]
        public string? Description { get; set; }
    }
}
