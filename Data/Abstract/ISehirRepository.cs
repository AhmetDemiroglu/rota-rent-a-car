using Rent_A_Car.Entity;
namespace Rent_A_Car.Data.Abstract
{
    public interface ISehirRepository
    {
        IQueryable<Sehir> Sehirler { get; }
        void SehirEkle(Sehir sehir);
        void SehirDuzenle(Sehir sehir);
        void SehirSil(Sehir sehir);
    }
}
