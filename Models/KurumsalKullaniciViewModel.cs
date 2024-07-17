using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Rent_A_Car.Models
{
    public class KurumsalKullaniciViewModel
    {
        public int KurumsalKullaniciId { get; set; }
        [Required(ErrorMessage = "*Eposta zorunludur.")]
        [EmailAddress]
        [Display(Name = "Eposta")]
        public string? FirmaEposta { get; set; }
        [Required(ErrorMessage = "*Parola zorunludur.")]
        [StringLength(10, ErrorMessage = "En fazla {1}, en az {2} karakter girilmelidir.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string? FirmaSifre { get; set; }
        [Required(ErrorMessage = "*Parola Tekrar zorunludur.")]
        [DataType(DataType.Password)]
        [Compare(nameof(FirmaSifre), ErrorMessage = "Girilen parolalar eşleşmedi.")]
        [Display(Name = "Parola Tekrar")]
        public string? FirmaSifreKontrol { get; set; }
        public string? FirmaAdi { get; set; }
        public string? VergiNo { get; set; }
        public string? VergiDairesi { get; set; }
        public string? FirmaIl { get; set; }
        public string? FirmaIlce { get; set; }
        public string? VergiIl { get; set; }
        public string? VergiIlce { get; set; }
        public string? FirmaTelefon { get; set; }
        public string? FaturaAdresi { get; set; }
    }
}
