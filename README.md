# Rota Rent a Car - ASP.NET Core MVC Araç Kiralama Projesi

Bu proje, ASP.NET Core MVC kullanılarak geliştirilmiş, temel CRUD (Oluşturma, Okuma, Güncelleme, Silme) işlevselliklerine sahip tam kapsamlı bir araç kiralama web uygulamasıdır. Proje, modern .NET teknolojilerini ve en iyi geliştirme pratiklerini sergilemek amacıyla bir portfolyo çalışması olarak hazırlanmıştır.

## 🚗 Projenin Amacı

**Rota Rent a Car**, kullanıcıların sisteme kaydolup giriş yapabildiği, mevcut araçları listeleyebildiği, araç detaylarını görüntüleyebildiği ve yönetici rolündeki kullanıcıların araç, marka gibi varlıkları yönetebildiği bir platform sunar. Bu proje, C# ve .NET ekosistemindeki yetkinlikleri pratik bir örnek üzerinden göstermeyi hedefler.

## 🛠️ Kullanılan Teknolojiler

Projenin geliştirilmesinde aşağıdaki teknolojiler ve kütüphaneler kullanılmıştır:

* **Backend:** ASP.NET Core MVC (.NET 8)
* **Veritabanı Erişimi:** Entity Framework Core
* **Kimlik Doğrulama ve Yetkilendirme:** ASP.NET Core Identity
* **Veritabanı:** Microsoft SQL Server
* **Frontend:** Razor Pages, HTML5, CSS3, Bootstrap

## ✨ Öne Çıkan Özellikler

* **Kullanıcı Yönetimi:** Güvenli kayıt (register) ve giriş (login) sistemi.
* **Rol Tabanlı Yetkilendirme:** Sadece "Admin" rolüne sahip kullanıcıların kritik işlemleri (araç ekleme/silme/güncelleme) yapabilmesi.
* **Araç Yönetimi:** Araçları listeleme, detaylarını görme, yeni araç ekleme, mevcut araçları güncelleme ve silme.
* **Marka Yönetimi:** Araç markalarını yönetmek için tam CRUD desteği.
* **İlişkisel Veri Yapısı:** Araçlar ve markalar arasında kurulan ilişkisel (one-to-many) veritabanı şeması.
* **Temiz ve Sürdürülebilir Kod:** Standart MVC mimari desenlerine uygun, anlaşılır ve bakımı kolay kod yapısı.

## 🚀 Projeyi Yerel Makinede Çalıştırma

Projeyi kendi bilgisayarınızda çalıştırmak için aşağıdaki adımları izleyebilirsiniz:

1.  **Repoyu Klonlama:**
    ```bash
    git clone [https://github.com/AhmetDemiroglu/rota-rent-a-car.git](https://github.com/AhmetDemiroglu/rota-rent-a-car.git)
    ```

2.  **Gerekli Yazılımlar:**
    * [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
    * [Visual Studio 2022](https://visualstudio.microsoft.com/tr/) veya uyumlu bir IDE.
    * [SQL Server](https://www.microsoft.com/tr-tr/sql-server/sql-server-downloads) (Express, Developer veya tam sürüm).

3.  **Veritabanı Ayarları:**
    * Proje içerisindeki `appsettings.json` dosyasını açın.
    * `ConnectionStrings` bölümündeki `DefaultConnection` değerini kendi SQL Server bağlantı bilgilerinizle güncelleyin.
        ```json
        "ConnectionStrings": {
          "DefaultConnection": "Server=SUNUCU_ADINIZ;Database=RotaRentACarDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
        }
        ```
    * Visual Studio'da **Package Manager Console**'u açın ve aşağıdaki komutu çalıştırarak veritabanını oluşturun ve şemayı uygulayın:
        ```powershell
        Update-Database
        ```

4.  **Uygulamayı Başlatma:**
    * Projeyi Visual Studio üzerinden `F5` tuşuna basarak veya `dotnet run` komutu ile başlatın.
    * Uygulama başladığında, ilk olarak bir kullanıcı kaydı oluşturun. Veritabanında varsayılan bir admin rolü veya kullanıcısı bulunmamaktadır.
