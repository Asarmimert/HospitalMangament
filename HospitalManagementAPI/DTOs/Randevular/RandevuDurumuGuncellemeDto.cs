using HospitalManagement.Entity.Enums;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Randevular
{
    public class RandevuDurumGuncellemeDto
    {
        [Required(ErrorMessage = "Randevu durumu zorunludur.")]
        public RandevuDurumu Durum { get; set; }

        [MaxLength(
            300,
            ErrorMessage = "İptal nedeni en fazla 300 karakter olabilir.")]
        public string? IptalNedeni { get; set; }
    }
}