using System.ComponentModel.DataAnnotations;

namespace Rent_A_Car.Models
{
    public class BireyselLoginViewModel
    {
        public int BireyselKullaniciId { get; set; }
        [Required(ErrorMessage = "*Email zorunludur.")]
        [EmailAddress]
        [Display(Name = "Eposta")]
        public string? Eposta { get; set; }
        [Required(ErrorMessage = "*Parola zorunludur.")]
        [StringLength(10, ErrorMessage = "En fazla {1}, en az {2} karakter girilmelidir.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string? Sifre { get; set; }
    }
}
