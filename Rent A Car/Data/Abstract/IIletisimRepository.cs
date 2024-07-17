using Rent_A_Car.Entity;

namespace Rent_A_Car.Data.Abstract
{
    public interface IIletisimRepository
    {
        IQueryable<Iletisim> Iletisimler { get; }
        void MesajEkle(Iletisim mesaj);
        void MesajGuncelle(Iletisim mesaj);
        void MesajSil(Iletisim mesaj);
    }
}
