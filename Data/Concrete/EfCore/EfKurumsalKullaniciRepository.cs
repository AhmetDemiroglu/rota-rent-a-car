
using Rent_A_Car.Entity;
using Rent_A_Car.Data.Abstract;
using Rent_A_Car.Models;

namespace Rent_A_Car.Data.Concrete.EfCore
{
    public class EfKurumsalKullaniciRepository : IKurumsalKullaniciRepository
    {
        private readonly Context _context;
        public EfKurumsalKullaniciRepository(Context context)
        {
            _context = context;
        }
        public IQueryable<KurumsalKullanici> KurumsalKullanicilar => _context.KurumsalKullanicilar;
        public void KullaniciDuzenle(KurumsalKullanici kullanici)
        {
            var entity = _context.KurumsalKullanicilar.FirstOrDefault(i => i.KurumsalKullaniciId == kullanici.KurumsalKullaniciId);
            if (entity != null)
            {
                entity.FirmaAdi = kullanici.FirmaAdi;
                entity.FirmaEposta = kullanici.FirmaEposta;
                entity.FirmaSifre = kullanici.FirmaSifre;
                entity.FirmaSifreKontrol = kullanici.FirmaSifreKontrol;
                entity.FirmaTelefon = kullanici.FirmaTelefon;
                entity.FaturaAdresi = kullanici.FaturaAdresi;
                entity.FirmaIl = kullanici.FirmaIl;
                entity.FirmaIlce = kullanici.FirmaIlce;
                entity.VergiNo = kullanici.VergiNo;
                entity.VergiDairesi = kullanici.VergiDairesi;
                entity.VergiIl = kullanici.VergiIl;
                entity.VergiIlce = kullanici.VergiIlce;

                _context.SaveChanges();
            }
        }

        public void KullaniciSil(KurumsalKullanici kullanici)
        {
            var entity = _context.KurumsalKullanicilar.FirstOrDefault(i => i.KurumsalKullaniciId == kullanici.KurumsalKullaniciId);
            if (entity != null)
            {
                _context.Remove(entity);
                _context.SaveChanges();
            }
        }

        public bool KullaniciDurumu(string eposta)
        { 
            var kullaniciEpostasıMevcut = _context.KurumsalKullanicilar.Any(u => u.FirmaEposta == eposta);
            return kullaniciEpostasıMevcut;
        }
        public void KullaniciKayit(KurumsalKullanici kullanici)
        {
            _context.Add(kullanici);
            _context.SaveChanges();
        }
    }
}
