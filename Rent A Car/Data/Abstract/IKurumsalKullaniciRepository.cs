using Rent_A_Car.Entity;
using Rent_A_Car.Models;

namespace Rent_A_Car.Data.Abstract
{
    public interface IKurumsalKullaniciRepository
    {
        IQueryable<KurumsalKullanici> KurumsalKullanicilar { get; }
        void KullaniciKayit(KurumsalKullanici kullanici);
        void KullaniciDuzenle(KurumsalKullanici kullanici);
        void KullaniciSil(KurumsalKullanici kullanici);
        bool KullaniciDurumu(string eposta);
    }
}
