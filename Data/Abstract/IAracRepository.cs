
using Rent_A_Car.Entity;

namespace Rent_A_Car.Data.Abstract
{
    public interface IAracRepository
    {
        IQueryable <Arac> Araclar { get; }
        void AracEkle(Arac arac);
        void AracDuzenle(Arac arac);
        void AracSil(Arac arac);
        IEnumerable<Arac> GetAvailableCars(DateTime teslimTarihi, DateTime iadeTarihi);
        IEnumerable<Sehir> GetCitiesByCarId(int aracId);
        IEnumerable<Arac> GetAvailableCarsByLocation(DateTime teslimTarihi, DateTime iadeTarihi, int teslimYeriId);
        void GuncelleAracKonumu(int aracId, int yeniSehirId);

    }
}
