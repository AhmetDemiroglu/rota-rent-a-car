using Rent_A_Car.Entity;
using Rent_A_Car.Models;

namespace Rent_A_Car.Data.Abstract
{
    public interface IBireyselKullaniciRepository
    {
        IQueryable<BireyselKullanici> BireyselKullanicilar { get; }
        void KullaniciKayit(BireyselKullanici kullanici);
        void KullaniciDuzenle(BireyselKullanici kullanici);
        void KullaniciSil(BireyselKullanici kullanici);
        bool KullaniciDurumu(string eposta);
    }
}
