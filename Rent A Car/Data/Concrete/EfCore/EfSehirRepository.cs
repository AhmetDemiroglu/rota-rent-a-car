using Rent_A_Car.Data.Abstract;
using Rent_A_Car.Entity;

namespace Rent_A_Car.Data.Concrete.EfCore
{
    public class EfSehirRepository:ISehirRepository
    {
        private readonly Context _context;   
        public EfSehirRepository(Context context)
        {
            _context = context;
        }
        public IQueryable<Sehir> Sehirler => _context.Sehirler;
        public void SehirEkle(Sehir sehir)
        {
            _context.Add(sehir);
            _context.SaveChanges();
        }
        public void SehirDuzenle (Sehir sehir) 
        {
            var entity = _context.Sehirler.FirstOrDefault(i=> i.SehirId == sehir.SehirId);
            if (entity != null)
            {
                entity.IlceAdi = sehir.IlceAdi;
                entity.IlAdi = sehir.IlAdi;
            }
        }
        public void SehirSil(Sehir sehir)
        {
            var entity = _context.Sehirler.FirstOrDefault(i => i.SehirId == sehir.SehirId);
            if (entity != null)
            {
                _context.Remove(entity);
                _context.SaveChanges();
            }
        }
    }
}
