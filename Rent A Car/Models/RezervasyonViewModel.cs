using Microsoft.Extensions.Hosting;
using Rent_A_Car.Entity;

namespace Rent_A_Car.Models
{
    public class RezervasyonViewModel
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
        public Arac Arac { get; set; } = new Arac();
        public decimal Fiyat { get; set; }
        public List<Rezervasyon> Rezervasyonlar { get; set; } = new();
        public List<Arac> Araclar { get; set; } = new();
        public int? KullaniciId { get; set; }
        public int BireyselKullaniciId { get; set; }
        public int KurumsalKullaniciId { get; set; }
        public BireyselKullanici BireyselKullanici { get; set; } = new BireyselKullanici();
        public KurumsalKullanici KurumsalKullanici { get; set; } = new KurumsalKullanici();

    }
}
