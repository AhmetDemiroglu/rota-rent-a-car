namespace Rent_A_Car.Entity
{
    public class Sehir
    {
        public int SehirId { get; set; }
        public string? IlAdi { get; set; }
        public string? IlceAdi { get; set; }
        public List<Arac> Araclar { get; set; } = new List<Arac>();

    }
}
