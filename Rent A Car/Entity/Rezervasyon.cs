namespace Rent_A_Car.Entity
{
    public class Rezervasyon
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
        public Arac? Arac { get; set; }
        public int? BireyselKullaniciId { get; set; }
        public BireyselKullanici? BireyselKullanici { get; set; }
        public int? KurumsalKullaniciId { get; set; }
        public KurumsalKullanici? KurumsalKullanici { get; set; }
        public decimal Fiyat { get; set; }
        public Sehir? Sehir { get; set; }
    }
}
