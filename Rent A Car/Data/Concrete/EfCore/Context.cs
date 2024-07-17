
using Microsoft.EntityFrameworkCore;
using Rent_A_Car.Entity;

namespace Rent_A_Car.Data.Concrete.EfCore
{
    public class Context:DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<Arac> Araclar => Set<Arac>();
        public DbSet<Iletisim> Iletisimler => Set<Iletisim>();
        public DbSet<Kategori> Kategoriler => Set<Kategori>();
        public DbSet<BireyselKullanici> BireyselKullanicilar => Set<BireyselKullanici>();
        public DbSet<KurumsalKullanici> KurumsalKullanicilar => Set<KurumsalKullanici>();
        public DbSet<Rezervasyon> Rezervasyonlar => Set<Rezervasyon>();
        public DbSet<Sehir> Sehirler => Set<Sehir>();

    }
}
