Rent-A-Car Web Uygulaması
Bu proje, Rent A Car hizmeti veren bir web uygulamasıdır. Proje, kullanıcılara araç kiralama, rezervasyon yönetimi, ve araç bilgilerini görüntüleme gibi işlevler sunar. Admin paneli ile yönetim kolaylığı sağlar ve kullanıcılara modern, kullanıcı dostu bir arayüz sunar.

Proje Gereksinimleri:
.NET 7 SDK
Visual Studio 2022
SQL Server
Entity Framework Core

Kurulum Adımları:
Projeyi Kopyalayın

'
git clone https://github.com/username/rent-a-car.git
cd rent-a-car
Paket Yöneticisi Konsolu'nu Açın
'

Visual Studio'da projeyi açtıktan sonra, üst menüden Araçlar > NuGet Paket Yöneticisi > Paket Yöneticisi Konsolu seçeneğine gidin.

Veritabanı Bağlantısı Ayarları
'
appsettings.json dosyasındaki ConnectionStrings bölümünde veritabanı bağlantı ayarlarını yapılandırın:
'

"json"
'
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RentACarDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
'

Migration Oluşturma ve Veritabanını Güncelleme

Paket Yöneticisi Konsolu'nda aşağıdaki komutları sırasıyla çalıştırarak migration işlemini gerçekleştirin ve veritabanını güncelleyin:

Migration oluşturmak için:

'
Add-Migration InitialCreate
Veritabanını güncellemek için:
'

'
Update-Database
Uygulamayı Çalıştırın Visual Studio’da F5 tuşuna basarak uygulamayı başlatın. Uygulama, varsayılan olarak https://localhost:5001 adresinde çalışacaktır.
'

Proje Yapısı:
Data: Veritabanı işlemleri ve EF Core repository sınıfları burada bulunur.
Controllers: MVC modelinde yer alan kontrol sınıfları burada tanımlıdır.
Views: Razor View dosyalarının bulunduğu dizindir.
wwwroot: Statik dosyalar (CSS, JS, resimler) burada bulunur.

Teknolojiler:
.NET Core 7
Entity Framework Core
Bootstrap
SQLite (Geliştirme aşaması için) veya SQL Server

Özellikler:
Admin Paneli: Yöneticiler, araç ve kullanıcı yönetimi gibi işlemleri gerçekleştirebilir.
Kullanıcı Yönetimi: Bireysel ve kurumsal kullanıcılar sisteme kayıt olup araç rezervasyonu yapabilir.
Araç Yönetimi: Yöneticiler araç ekleyebilir, düzenleyebilir ve silebilir.
Rezervasyon Sistemi: Kullanıcılar, araç kiralama rezervasyonlarını yapabilir ve yönetebilir.

Katkıda Bulunma:
Katkıda bulunmak isterseniz lütfen bir pull request gönderin veya bir issue açın.
