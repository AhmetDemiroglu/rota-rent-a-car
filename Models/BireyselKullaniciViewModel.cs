using System.ComponentModel.DataAnnotations;

namespace Rent_A_Car.Models
{
    public class BireyselKullaniciViewModel
    {
        public int BireyselKullaniciId { get; set; }
        public string? Ad { get; set; }
        public string? Soyad { get; set; }
        public string AdSoyad => $"{Ad} {Soyad}".Trim();
        [Required(ErrorMessage = "*Eposta zorunludur.")]
        [EmailAddress]
        [Display(Name = "Eposta")]
        public string? Eposta { get; set; }
        public string? Adres { get; set; }
        public string? Telefon { get; set; }
        [Required(ErrorMessage = "*Parola zorunludur.")]
        [StringLength(10, ErrorMessage = "En fazla {1}, en az {2} karakter girilmelidir.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string? Sifre { get; set; }
        [Required(ErrorMessage = "*Parola Tekrar zorunludur.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Sifre), ErrorMessage = "Girilen parolalar eşleşmedi.")]
        [Display(Name = "Parola Tekrar")]
        public string? SifreKontrol { get; set; }
        public string? KimlikNo { get; set; }
        public DateTime DogumTarihi { get; set; }
    }
}
