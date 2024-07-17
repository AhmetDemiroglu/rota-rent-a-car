using Microsoft.AspNetCore.Identity;

namespace Rent_A_Car.Entity
{
    public class BireyselKullanici
    {
        public int BireyselKullaniciId { get; set; }
        public string? Ad { get; set; }
        public string? Soyad { get; set; }
        public string? Eposta { get; set; }
        public string? Adres { get; set; }
        public string? Telefon { get; set; }
        public string? Sifre { get; set;}
        public string? SifreKontrol { get; set; }
        public string? KimlikNo { get; set; }
        public DateTime DogumTarihi { get; set; }
    }
}
