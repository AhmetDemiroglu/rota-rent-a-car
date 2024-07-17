using Rent_A_Car.Data.Abstract;
using Rent_A_Car.Entity;

namespace Rent_A_Car.Data.Concrete.EfCore
{
    public class EfRezervasyonRepository : IRezervasyonRepository
    {
        private Context _context;

        public EfRezervasyonRepository(Context context)
        {
            _context = context;
        }

        public IQueryable<Rezervasyon> Rezervasyonlar => _context.Rezervasyonlar;
        public void RezervasyonDuzenle(Rezervasyon rezervasyon)
        {
            var entity = _context.Rezervasyonlar.FirstOrDefault(i => i.RezervasyonId == rezervasyon.RezervasyonId);
            if (entity != null)
            {
                entity.Fiyat = rezervasyon.Fiyat;
                entity.Sigorta = rezervasyon.Sigorta;
                entity.TeslimTarih = rezervasyon.TeslimTarih;
                entity.IadeTarih = rezervasyon.IadeTarih;
                entity.IadeYeriId = rezervasyon.IadeYeriId;
                entity.TeslimYeriId = rezervasyon.TeslimYeriId;
                entity.IsActive = rezervasyon.IsActive;
                entity.TeslimYeriAdi = rezervasyon.TeslimYeriAdi;
                entity.IadeYeriAdi = rezervasyon.IadeYeriAdi;  // Bu satırı ekleyelim.
                _context.SaveChanges();
            }
        }

        public void RezervasyonEkle(Rezervasyon rezervasyon) 
        {
            _context.Add(rezervasyon);
            _context.SaveChanges();
        }

        public void RezervasyonSil(Rezervasyon rezervasyon)
        {
            var entity = _context.Rezervasyonlar.FirstOrDefault(i => i.RezervasyonId == rezervasyon.RezervasyonId);
            if (entity != null)
            {
                _context.Remove(entity);
                _context.SaveChanges();
            }
        }
        public bool RezervasyonDurumu(int aracId, DateTime teslimTarihi, DateTime iadeTarihi)
        {
            // Belirtilen araç ID'sine ve tarih aralığına göre rezervasyon olup olmadığını kontrol eder.
            return !_context.Rezervasyonlar.Any(r =>
                r.AracId == aracId && // Araç ID'sine göre filtreleme yapar.
                r.TeslimTarih < iadeTarihi && // Teslim tarihi, belirtilen iade tarihinden küçük olmalıdır.
                r.IadeTarih > teslimTarihi // İade tarihi, belirtilen teslim tarihinden büyük olmalıdır.
            );
        }
        public bool RezervasyonDuzenlemeSorgu(int rezervasyonId, int aracId, DateTime teslimTarihi, DateTime iadeTarihi)
        {
            return !_context.Rezervasyonlar.Any(r =>
                r.RezervasyonId != rezervasyonId && // Mevcut düzenlenen rezervasyonu dışarıda tut
                r.AracId == aracId &&
                r.TeslimTarih < iadeTarihi &&
                r.IadeTarih > teslimTarihi
            );
        }
        public bool RezervasyonDurumuGuncelle()
        {
            // İade tarihi geçmiş ve tamamlanmamış rezervasyonları alır.
            var rezervasyonlar = _context.Rezervasyonlar.Where(r => r.IadeTarih < DateTime.Now && !r.IsCompleted).ToList();
            if (rezervasyonlar.Any())
            {
                // Her bir rezervasyonu tamamlanmış olarak işaretler.
                foreach (var rezervasyon in rezervasyonlar)
                {
                    rezervasyon.IsCompleted = true;
                }
                // Değişiklikleri veritabanına kaydeder.
                _context.SaveChanges();
                return true;
            }
            return false;
        }

    }
}
