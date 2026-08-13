using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Ilaclar
{
    public class IlacOlusturmaDto
    {
        [Required(ErrorMessage = "İlaç adı zorunludur.")]
        [MaxLength(
            50,
            ErrorMessage = "İlaç adı en fazla 50 karakter olabilir.")]
        public string Ad { get; set; } = string.Empty;
    }
}