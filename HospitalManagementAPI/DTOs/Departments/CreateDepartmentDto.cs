using System.ComponentModel.DataAnnotations;
namespace HospitalManagementAPI.DTOs.Departments
{
    public class CreateDepartmentDto
    {
        [Required(ErrorMessage = "Bölüm Adınızı Giriniz:")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;



        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
