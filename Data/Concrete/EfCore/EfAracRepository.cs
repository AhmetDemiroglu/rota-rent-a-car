using Rent_A_Car.Data.Abstract;
using Rent_A_Car.Entity;
using Microsoft.EntityFrameworkCore;


namespace Rent_A_Car.Data.Concrete.EfCore
{
    public class EfAracRepository : IAracRepository
    {
        private Context _context;
        private IWebHostEnvironment _env;

        public EfAracRepository(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IQueryable<Arac> Araclar => _context.Araclar.Include(a => a.Kategori).Include(a => a.Sehir);

        public void AracEkle(Arac arac)
        {
            _context.Add(arac);
            _context.SaveChanges();
        }

        public void AracDuzenle(Arac arac)
        {
            var entity = _context.Araclar.FirstOrDefault(i => i.AracId == arac.AracId);
            if (entity != null)
            {
                entity.Marka = arac.Marka;
                entity.AracModel = arac.AracModel;
                entity.Vites = arac.Vites;
                entity.Motor = arac.Motor;
                entity.Koltuk = arac.Koltuk;
                entity.Renk = arac.Renk;
                entity.Resim = arac.Resim;
                entity.Plaka = arac.Plaka;
                entity.Km = arac.Km;
                entity.Musaitlik = arac.Musaitlik;
                entity.AracFiyat = arac.AracFiyat;
                entity.KategoriId = arac.KategoriId;
                entity.SehirId = arac.SehirId;
                _context.SaveChanges();
            }
        }

        public void AracSil(Arac arac)
        {
            if (!string.IsNullOrEmpty(arac.Resim))
            {
                var ResimDosyasi = Path.Combine(_env.WebRootPath, "img", arac.Resim);
                if (System.IO.File.Exists(ResimDosyasi))
                {
                    System.IO.File.Delete(ResimDosyasi);
                }
            }

            var entity = _context.Araclar.FirstOrDefault(i => i.AracId == arac.AracId);
            if (entity != null)
            {
                _context.Remove(entity);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Arac> GetAvailableCars(DateTime teslimTarihi, DateTime iadeTarihi)
        {
            // Öncelikle, belirtilen tarih aralığında (teslimTarihi ve iadeTarihi) rezerve edilmiş araçların ID'lerini alıyoruz.
            var reservedCars = _context.Rezervasyonlar
                .Where(r => r.TeslimTarih < iadeTarihi && r.IadeTarih > teslimTarihi) // Teslim tarihi iade tarihinden küçük ve iade tarihi teslim tarihinden büyük olan rezervasyonları filtreler
                .Select(r => r.AracId) // Bu rezervasyonların araç ID'lerini seçer
                .ToList(); // Listeye dönüştürür

            // Daha sonra, rezerve edilmiş araçların ID'lerini içermeyen ve mevcut (musait) olan araçları sorguluyoruz.
            return _context.Araclar
                .Where(a => !reservedCars.Contains(a.AracId) && a.Musaitlik == true) // Rezerve edilmemiş ve musait olan araçları filtreler
                .Include(a => a.Sehir) // Araçların bulunduğu şehir bilgilerini dahil eder
                .ToList(); // Sonucu listeye dönüştürür
        }
        public IEnumerable<Arac> GetAvailableCarsByLocation(DateTime teslimTarihi, DateTime iadeTarihi, int teslimYeriId)
        {
            // Öncelikle, belirtilen tarih aralığında (teslimTarihi ve iadeTarihi) rezerve edilmiş araçların ID'lerini alıyoruz.
            var reservedCars = _context.Rezervasyonlar
                .Where(r => r.TeslimTarih < iadeTarihi && r.IadeTarih > teslimTarihi) // Teslim tarihi iade tarihinden küçük ve iade tarihi teslim tarihinden büyük olan rezervasyonları filtreler
                .Select(r => r.AracId) // Bu rezervasyonların araç ID'lerini seçer
                .ToList(); // Listeye dönüştürür

            // Daha sonra, rezerve edilmiş araçların ID'lerini içermeyen, belirtilen şehirde olan ve mevcut (musait) olan araçları sorguluyoruz.
            return _context.Araclar
                .Where(a => !reservedCars.Contains(a.AracId) && a.SehirId == teslimYeriId && a.Musaitlik == true) // Rezerve edilmemiş, belirtilen şehirde ve musait olan araçları filtreler
                .Include(a => a.Sehir) // Araçların bulunduğu şehir bilgilerini dahil eder
                .ToList(); // Sonucu listeye dönüştürür
        }
        public IEnumerable<Sehir> GetCitiesByCarId(int aracId)
        {
            // Öncelikle, belirtilen araç ID'sine ve musaitlik durumuna göre araçları filtreliyoruz.
            return _context.Araclar
                .Where(a => a.AracId == aracId && a.Musaitlik == true) // Belirtilen araç ID'sine ve musaitlik durumuna göre filtreler
                .Include(a => a.Sehir) // Araçların bulunduğu şehir bilgilerini dahil eder
                .Select(a => a.Sehir) // Şehir bilgilerini seçer
                .Distinct() // Aynı şehirleri tekrar etmeyecek şekilde filtreler
                .ToList(); // Sonucu listeye dönüştürür
        }
        public void GuncelleAracKonumu(int aracId, int yeniSehirId)
        {
            var entity = _context.Araclar.FirstOrDefault(i => i.AracId == aracId);
            if (entity != null)
            {
                // Araç konumunu yeni şehir ID'si ile günceller.
                entity.SehirId = yeniSehirId;
                _context.SaveChanges();
            }
        }
    }
}