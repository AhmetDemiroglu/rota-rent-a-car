using Rent_A_Car.Entity;

namespace Rent_A_Car.Data.Abstract
{
    public interface IRezervasyonRepository
    {
        IQueryable<Rezervasyon> Rezervasyonlar { get; }
        void RezervasyonEkle(Rezervasyon rezervasyon);
        void RezervasyonDuzenle(Rezervasyon rezervasyon);
        void RezervasyonSil(Rezervasyon rezervasyon);
        bool RezervasyonDurumu(int aracId, DateTime teslimTarihi, DateTime iadeTarihi);
        bool RezervasyonDuzenlemeSorgu(int rezervasyonId, int aracId, DateTime teslimTarihi, DateTime iadeTarihi);
        bool RezervasyonDurumuGuncelle(); 
    }
}
