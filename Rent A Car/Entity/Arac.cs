namespace Rent_A_Car.Entity
{
    public class Arac
    {
        public int AracId { get; set; }
        public string? Marka { get; set; }
        public string? AracModel { get; set; }
        public string? Vites { get; set; }
        public string? Motor { get; set;}
        public string? Koltuk { get; set; }
        public string? Renk { get;set; }
        public string? Resim { get;set; }
        public string? Plaka { get;set; }
        public int Km { get; set; }
        public bool? Musaitlik {  get;set; }
        public int KategoriId { get; set; }
        public Kategori Kategori { get; set; } = null!;
        public int SehirId { get; set; }
        public Sehir Sehir { get; set; } = null!;
        public decimal AracFiyat { get; set; }
    }
}
