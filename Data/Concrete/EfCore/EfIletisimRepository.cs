using Rent_A_Car.Data.Abstract;
using Rent_A_Car.Entity;

namespace Rent_A_Car.Data.Concrete.EfCore
{
    public class EfIletisimRepository:IIletisimRepository
    {
        private readonly Context _context;
        public EfIletisimRepository(Context context)
        {
            _context = context;
        }

        public IQueryable<Iletisim> Iletisimler => _context.Iletisimler;

        public void MesajEkle(Iletisim mesaj)
        {
            _context.Add(mesaj);
            _context.SaveChanges();
        }

        public void MesajGuncelle(Iletisim mesaj)
        {
            _context.Update(mesaj);
            _context.SaveChanges();
        }

        public void MesajSil(Iletisim mesaj)
        {
            _context.Remove(mesaj);
            _context.SaveChanges();
        }

    }
}
