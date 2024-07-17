
using Rent_A_Car.Entity;
using Rent_A_Car.Data.Abstract;
using Rent_A_Car.Models;

namespace Rent_A_Car.Data.Concrete.EfCore
{
    public class EfBireyselKullaniciRepository: IBireyselKullaniciRepository
    {
        private readonly Context _context;
        public EfBireyselKullaniciRepository(Context context)
        {
            _context = context;
        }
        public IQueryable<BireyselKullanici> BireyselKullanicilar => _context.BireyselKullanicilar;
        public void KullaniciDuzenle(BireyselKullanici kullanici)
        {
            var entity = _context.BireyselKullanicilar.FirstOrDefault(i => i.BireyselKullaniciId == kullanici.BireyselKullaniciId);
            if (entity != null)
            {
                entity.Ad = kullanici.Ad;
                entity.Soyad = kullanici.Soyad;
                entity.Eposta = kullanici.Eposta;
                entity.Adres = kullanici.Adres;
                entity.Telefon = kullanici.Telefon;
                entity.Sifre = kullanici.Sifre;
                entity.SifreKontrol = kullanici.SifreKontrol;
                entity.KimlikNo = kullanici.KimlikNo;
                entity.DogumTarihi = kullanici.DogumTarihi;
                _context.SaveChanges();
            }
        }

        public void KullaniciSil(BireyselKullanici kullanici)
        {
            var entity = _context.BireyselKullanicilar.FirstOrDefault(i => i.BireyselKullaniciId == kullanici.BireyselKullaniciId);
            if (entity != null)
            {
                _context.Remove(entity);
                _context.SaveChanges();
            }
        }

        public bool KullaniciDurumu(string eposta)
        { 
            var kullaniciEpostasıMevcut = _context.BireyselKullanicilar.Any(u => u.Eposta == eposta);
            return kullaniciEpostasıMevcut;
        }
        public void KullaniciKayit(BireyselKullanici kullanici)
        {
            _context.Add(kullanici);
            _context.SaveChanges();
        }
    }
}
