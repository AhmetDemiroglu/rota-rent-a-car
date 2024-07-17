using Rent_A_Car.Entity;

namespace Rent_A_Car.Data.Abstract
{
    public interface IKategoriRepository
    {
        IQueryable<Kategori> Kategoriler { get; }
        void KategoriEkle(Kategori kategori);
        void KategoriDuzenle(Kategori kategori);
        void KategoriSil(Kategori kategori);

    }
}
