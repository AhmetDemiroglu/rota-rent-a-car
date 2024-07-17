using Rent_A_Car.Data.Abstract;
using Rent_A_Car.Entity;

namespace Rent_A_Car.Data.Concrete.EfCore
{
    public class EfKategoriRepository : IKategoriRepository
    {

        private Context _context;
        private IWebHostEnvironment _env;

        public EfKategoriRepository(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IQueryable<Kategori> Kategoriler => _context.Kategoriler;

        public void KategoriEkle(Kategori kategori)
        {
            _context.Add(kategori);
            _context.SaveChanges();
        }
        public void KategoriDuzenle(Kategori kategori)
        {
            var entity = _context.Kategoriler.FirstOrDefault(i => i.KategoriId == kategori.KategoriId);
            if (entity != null)
            {
                entity.KategoriAdi = kategori.KategoriAdi;
                _context.SaveChanges();
            }
        }
        public void KategoriSil(Kategori kategori)
        {
            var entity = _context.Kategoriler.FirstOrDefault(i => i.KategoriId == kategori.KategoriId);
            if (entity != null)
            {
                _context.Remove(entity);
                _context.SaveChanges();
            }
        }
    }
}
