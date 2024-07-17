namespace Rent_A_Car.Entity
{
    public class Kategori
    {
        public int KategoriId { get; set; }
        public string? KategoriAdi { get; set; }
        public List<Arac> Araclar { get; set; } = new List<Arac>();
    }
}
