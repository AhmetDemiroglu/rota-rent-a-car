using Microsoft.AspNetCore.Mvc.Rendering;
using Rent_A_Car.Entity;

namespace Rent_A_Car.Models
{
    public class RezervasyonEkleViewModel
    {
        public int RezervasyonId { get; set; }
        public DateTime TeslimTarih { get; set; }
        public DateTime IadeTarih { get; set; }
        public int? TeslimYeriId { get; set; }
        public int? IadeYeriId { get; set; }
        public string? TeslimYeriAdi { get; set; }
        public string? IadeYeriAdi { get; set; }
        public bool Sigorta { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public int AracId { get; set; }
        public IEnumerable<SelectListItem> AracSecenekleri { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> TeslimYeriSecenekleri { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> IadeYeriSecenekleri { get; set; } = new List<SelectListItem>();
        public Arac? Arac { get; set; }
        public decimal Fiyat { get; set; }
        public int BireyselKullaniciId { get; set; }
        public int KurumsalKullaniciId { get; set; }
        public BireyselKullanici BireyselKullanici { get; set; } = new BireyselKullanici();
        public KurumsalKullanici KurumsalKullanici { get; set; } = new KurumsalKullanici();
        public string? SelectedKullaniciId { get; set; }
        public List<SelectListItem> KullaniciSecenekleri { get; set; } = new();

    }
}
