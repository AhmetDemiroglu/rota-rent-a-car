using Rent_A_Car.Entity;

namespace Rent_A_Car.Models
{
    public class KullaniciViewModel
    {
        public List<BireyselKullanici> BireyselKullanicilar { get; set; } = new();
        public List<KurumsalKullanici> KurumsalKullanicilar { get; set; } = new();
    }
}
