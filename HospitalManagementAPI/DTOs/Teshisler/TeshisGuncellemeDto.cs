using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Teshisler
{
    public class TeshisGuncellemeDto
    {
        [MaxLength(
            100,
            ErrorMessage = "Teşhis kodu en fazla 100 karakter olabilir.")]
        public string? TeshisKodu { get; set; }

        [Required(ErrorMessage = "Teşhis adı zorunludur.")]
        [MaxLength(
            250,
            ErrorMessage = "Teşhis adı en fazla 250 karakter olabilir.")]
        public string TeshisAdi { get; set; } =
            string.Empty;

        public string? Aciklama { get; set; }
    }
}