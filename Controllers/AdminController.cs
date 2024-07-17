using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rent_A_Car.Data.Abstract;
using Rent_A_Car.Data.Concrete.EfCore;
using Rent_A_Car.Entity;
using Rent_A_Car.Models;
using System.Security.Claims;

namespace Rent_A_Car.Controllers
{
    [Authorize(Roles = "admin")] // Sadece admin rolüne sahip kullanıcılar bu kontrolöre erişebilir.
    public class AdminController : Controller
    {
        private readonly IRezervasyonRepository _rezervasyonRepository;
        private IWebHostEnvironment _env;
        private readonly IAracRepository _aracRepository;
        private readonly ISehirRepository _sehirRepository;
        private readonly IBireyselKullaniciRepository _kullaniciRepository;
        private readonly IPasswordHasher<BireyselKullanici> _passwordHasher;
        private readonly IKurumsalKullaniciRepository _kurumsalkullaniciRepository;
        private readonly IPasswordHasher<KurumsalKullanici> _passwordHasherKurumsal;
        private readonly IKategoriRepository _kategoriRepository;
        private readonly IIletisimRepository _iletisimRepository;

        // AdminController constructor, bağımlılıkları enjekte eder.
        public AdminController(IRezervasyonRepository rezervasyonRepository, ISehirRepository sehirRepository, IAracRepository aracRepository, IBireyselKullaniciRepository kullaniciRepository, IPasswordHasher<BireyselKullanici> passwordHasher, IKurumsalKullaniciRepository kurumsalkullaniciRepository, IPasswordHasher<KurumsalKullanici> passwordHasherKurumsal, IKategoriRepository kategoriRepository, IIletisimRepository iletisimRepository, IWebHostEnvironment env)
        {
            _rezervasyonRepository = rezervasyonRepository;
            _sehirRepository = sehirRepository;
            _aracRepository = aracRepository;
            _kullaniciRepository = kullaniciRepository;
            _passwordHasher = passwordHasher;
            _passwordHasherKurumsal = passwordHasherKurumsal;
            _kurumsalkullaniciRepository = kurumsalkullaniciRepository;
            _kategoriRepository = kategoriRepository;
            _iletisimRepository = iletisimRepository;
            _env = env;
        }

        public IActionResult Index()
        {
            // Okunmamış mesajların sayısını ViewBag aracılığıyla View'a gönderir.
            ViewBag.UnreadMessagesCount = _iletisimRepository.Iletisimler.Count(m => !m.IsRead);
            return View();
        }

        public IActionResult Mesajlar()
        {
            // Tüm mesajları en son gönderilene göre sıralar ve IletisimViewModel'e dönüştürür.
            var mesajlar = _iletisimRepository.Iletisimler
                                .OrderByDescending(m => m.SentDate)
                        .Select(m => new IletisimViewModel
                        {
                            IletisimId = m.IletisimId,
                            IletisimAdSoyad = m.IletisimAdSoyad,
                            IletisimEposta = m.IletisimEposta,
                            Telefon = m.Telefon,
                            Mesaj = m.Mesaj,
                            SentDate = m.SentDate,
                            IsRead = m.IsRead
                        })
                        .ToList();

            // Okunmamış mesajların sayısını günceller.
            ViewBag.UnreadMessagesCount = _iletisimRepository.Iletisimler.Count(m => !m.IsRead);
            return View(mesajlar);
        }

        public IActionResult MesajDetay(int id)
        {
            // Mesaj ID'sine göre mesajı getirir.
            var mesaj = _iletisimRepository.Iletisimler.FirstOrDefault(m => m.IletisimId == id);
            if (mesaj == null)
            {
                return NotFound();
            }

            // Mesajı okundu olarak işaretler ve günceller.
            mesaj.IsRead = true;
            _iletisimRepository.MesajGuncelle(mesaj);

            // Mesaj detaylarını ViewModel'e ekler.
            var viewModel = new IletisimViewModel
            {
                IletisimId = mesaj.IletisimId,
                IletisimAdSoyad = mesaj.IletisimAdSoyad,
                IletisimEposta = mesaj.IletisimEposta,
                Telefon = mesaj.Telefon,
                Mesaj = mesaj.Mesaj,
                SentDate = mesaj.SentDate,
                IsRead = mesaj.IsRead
            };

            // Okunmamış mesajların sayısını günceller.
            ViewBag.UnreadMessagesCount = _iletisimRepository.Iletisimler.Count(m => !m.IsRead);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult OkunduOlarakIsaretle(int id)
        {
            // Mesaj ID'sine göre mesajı getirir ve okundu olarak işaretler.
            var mesaj = _iletisimRepository.Iletisimler.FirstOrDefault(m => m.IletisimId == id);
            if (mesaj != null)
            {
                mesaj.IsRead = true;
                _iletisimRepository.MesajGuncelle(mesaj);
            }
            // Okunmamış mesajların sayısını günceller.
            ViewBag.UnreadMessagesCount = _iletisimRepository.Iletisimler.Count(m => !m.IsRead);
            return RedirectToAction("Mesajlar");
        }

        [HttpPost]
        public IActionResult MesajSil(int id)
        {
            // Mesaj ID'sine göre mesajı getirir ve siler.
            var mesaj = _iletisimRepository.Iletisimler.FirstOrDefault(m => m.IletisimId == id);
            if (mesaj != null)
            {
                _iletisimRepository.MesajSil(mesaj);
            }
            // Okunmamış mesajların sayısını günceller.
            ViewBag.UnreadMessagesCount = _iletisimRepository.Iletisimler.Count(m => !m.IsRead);
            return RedirectToAction("Mesajlar");
        }

        public IActionResult UserIndex()
        {
            // Bireysel ve kurumsal kullanıcıları getirir.
            var bireyselKullanicilar = _kullaniciRepository.BireyselKullanicilar.ToList();
            var kurumsalKullanicilar = _kurumsalkullaniciRepository.KurumsalKullanicilar.ToList();

            // Kullanıcıları ViewModel'e ekleyip View'a gönderir.
            var model = new KullaniciViewModel
            {
                BireyselKullanicilar = bireyselKullanicilar,
                KurumsalKullanicilar = kurumsalKullanicilar
            };

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(BireyselKullaniciViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Eposta adresinin kullanım durumunu kontrol eder.
                if (_kullaniciRepository.KullaniciDurumu(model.Eposta!))
                {
                    ModelState.AddModelError("", "Bu e-posta zaten kullanılıyor.");
                    return View(model);
                }

                // Yeni bireysel kullanıcı oluşturur.
                var entity = new BireyselKullanici
                {
                    BireyselKullaniciId = model.BireyselKullaniciId,
                    Ad = model.Ad,
                    Soyad = model.Soyad,
                    Eposta = model.Eposta,
                    Adres = model.Adres,
                    Telefon = model.Telefon,
                    KimlikNo = model.KimlikNo,
                    DogumTarihi = model.DogumTarihi,
                };

                // Şifreyi hashler ve kaydeder.
                entity.Sifre = _passwordHasher.HashPassword(entity, model.Sifre);
                entity.SifreKontrol = entity.Sifre;

                _kullaniciRepository.KullaniciKayit(entity);
                return RedirectToAction("UserIndex");
            }
            return View(model);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Bireysel kullanıcıyı ID'ye göre getirir.
            var kullanici = _kullaniciRepository.BireyselKullanicilar.FirstOrDefault(i => i.BireyselKullaniciId == id);
            if (kullanici == null)
            {
                return NotFound();
            }
            // Kullanıcı bilgilerini ViewModel'e ekleyip View'a gönderir.
            return View(new BireyselKullaniciViewModel
            {
                BireyselKullaniciId = kullanici.BireyselKullaniciId,
                Ad = kullanici.Ad,
                Eposta = kullanici.Eposta,
                DogumTarihi = kullanici.DogumTarihi,
                Adres = kullanici.Adres,
                Sifre = kullanici.Sifre,
                SifreKontrol = kullanici.Sifre,
                Telefon = kullanici.Telefon,
                Soyad = kullanici.Soyad,
                KimlikNo = kullanici.KimlikNo
            });
        }

        [HttpPost]
        public IActionResult Edit(BireyselKullaniciViewModel kullanici, bool changePassword)
        {
            if (!changePassword)
            {
                // Şifre ve kimlik numarasını modelden çıkarır.
                ModelState.Remove("Sifre");
                ModelState.Remove("SifreKontrol");
                var currentUser = _kullaniciRepository.BireyselKullanicilar.FirstOrDefault(u => u.BireyselKullaniciId == kullanici.BireyselKullaniciId);
                if (currentUser != null)
                {
                    kullanici.Sifre = currentUser.Sifre; // Eski parolayı korur.
                    kullanici.SifreKontrol = currentUser.SifreKontrol; // Eski parola kontrolünü korur.
                    kullanici.KimlikNo = currentUser.KimlikNo; // Eski kimlik numarasını korur.
                }
            }

            if (ModelState.IsValid)
            {
                // Kullanıcıyı günceller.
                var userUpdate = new BireyselKullanici
                {
                    BireyselKullaniciId = kullanici.BireyselKullaniciId,
                    Ad = kullanici.Ad,
                    Soyad = kullanici.Soyad,
                    Eposta = kullanici.Eposta,
                    DogumTarihi = kullanici.DogumTarihi,
                    Adres = kullanici.Adres,
                    Sifre = kullanici.Sifre,
                    SifreKontrol = kullanici.SifreKontrol,
                    Telefon = kullanici.Telefon,
                    KimlikNo = kullanici.KimlikNo
                };

                if (changePassword)
                {
                    // Şifreyi günceller.
                    userUpdate.Sifre = _passwordHasher.HashPassword(userUpdate, kullanici.Sifre);
                    userUpdate.SifreKontrol = _passwordHasher.HashPassword(userUpdate, kullanici.SifreKontrol);
                }

                _kullaniciRepository.KullaniciDuzenle(userUpdate);
                return RedirectToAction("UserIndex");
            }
            return View(kullanici);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
    }
    // Kullanıcıyı ID'ye göre getirir.
    var user = await _kullaniciRepository.BireyselKullanicilar.FirstOrDefaultAsync(i => i.BireyselKullaniciId == id);
            if (user == null)
            {
                return NotFound();
}
// Kullanıcı bilgilerini ViewModel'e ekleyip View'a gönderir.
return View(new BireyselKullaniciViewModel
{
    BireyselKullaniciId = user.BireyselKullaniciId,
    Ad = user.Ad,
    Soyad = user.Soyad,
    Eposta = user.Eposta,
    Adres = user.Adres,
    Telefon = user.Telefon,
    Sifre = user.Sifre,
    SifreKontrol = user.SifreKontrol,
    KimlikNo = user.KimlikNo,
    DogumTarihi = user.DogumTarihi,
});
        }

        [HttpPost]
public async Task<IActionResult> Delete(int id)
        {
            // Kullanıcıyı ID'ye göre getirir ve siler.
            var entityDelete = await _kullaniciRepository.BireyselKullanicilar.FirstOrDefaultAsync(k => k.BireyselKullaniciId == id);

if (entityDelete == null)
{
    return NotFound();
}
_kullaniciRepository.KullaniciSil(entityDelete);
return RedirectToAction("UserIndex");
        }

        public IActionResult CreateKurumsal()
{
    // Boş bir KurumsalKullaniciViewModel oluşturur ve View'a gönderir.
    return View(new KurumsalKullaniciViewModel());
}

[HttpPost]
public IActionResult CreateKurumsal(KurumsalKullaniciViewModel model)
{
    if (ModelState.IsValid)
    {
        // Eposta adresinin kullanım durumunu kontrol eder.
        if (_kurumsalkullaniciRepository.KurumsalKullanicilar.Any(u => u.FirmaEposta == model.FirmaEposta))
        {
            ModelState.AddModelError("", "Bu e-posta zaten kullanılıyor.");
            return View(model);
        }

        // Yeni kurumsal kullanıcı oluşturur.
        var user = new KurumsalKullanici
        {
            FirmaEposta = model.FirmaEposta,
            FirmaAdi = model.FirmaAdi,
            VergiNo = model.VergiNo,
            VergiDairesi = model.VergiDairesi,
            FirmaIl = model.FirmaIl,
            FirmaIlce = model.FirmaIlce,
            VergiIl = model.VergiIl,
            VergiIlce = model.VergiIlce,
            FirmaTelefon = model.FirmaTelefon,
            FaturaAdresi = model.FaturaAdresi
        };

        // Şifreyi hashler ve kaydeder.
        user.FirmaSifre = _passwordHasherKurumsal.HashPassword(user, model.FirmaSifre);
        user.FirmaSifreKontrol = _passwordHasherKurumsal.HashPassword(user, model.FirmaSifreKontrol);

        _kurumsalkullaniciRepository.KullaniciKayit(user);
        return RedirectToAction("UserIndex");
    }
    return View(model);
}

public IActionResult KurumsalEdit(int? id)
{
    if (id == null)
    {
        return NotFound();
    }
    // Kurumsal kullanıcıyı ID'ye göre getirir.
    var kullanici = _kurumsalkullaniciRepository.KurumsalKullanicilar.FirstOrDefault(i => i.KurumsalKullaniciId == id);
    if (kullanici == null)
    {
        return NotFound();
    }
    // Kullanıcı bilgilerini ViewModel'e ekleyip View'a gönderir.
    return View(new KurumsalKullaniciViewModel
    {
        KurumsalKullaniciId = kullanici.KurumsalKullaniciId,
        FirmaEposta = kullanici.FirmaEposta,
        FirmaAdi = kullanici.FirmaAdi,
        FirmaSifre = kullanici.FirmaSifre,
        FirmaSifreKontrol = kullanici.FirmaSifreKontrol,
        FirmaTelefon = kullanici.FirmaTelefon,
        FaturaAdresi = kullanici.FaturaAdresi,
        FirmaIl = kullanici.FirmaIl,
        FirmaIlce = kullanici.FirmaIlce,
        VergiNo = kullanici.VergiNo,
        VergiDairesi = kullanici.VergiDairesi,
        VergiIl = kullanici.VergiIl,
        VergiIlce = kullanici.VergiIlce,
    });
}

[HttpPost]
public IActionResult KurumsalEdit(KurumsalKullaniciViewModel kullanici, bool changePassword)
{
    if (!changePassword)
    {
        // Şifre ve kimlik numarasını modelden çıkarır.
        ModelState.Remove("FirmaSifre");
        ModelState.Remove("FirmaSifreKontrol");
        var currentUser = _kurumsalkullaniciRepository.KurumsalKullanicilar.FirstOrDefault(u => u.KurumsalKullaniciId == kullanici.KurumsalKullaniciId);
        if (currentUser != null)
        {
            kullanici.FirmaSifre = currentUser.FirmaSifre; // Eski parolayı korur.
            kullanici.FirmaSifreKontrol = currentUser.FirmaSifreKontrol; // Eski parola kontrolünü korur.
            kullanici.VergiNo = currentUser.VergiNo; // Eski vergi numarasını korur.
        }
    }

    if (ModelState.IsValid)
    {
        // Kurumsal kullanıcıyı günceller.
        var userUpdate = new KurumsalKullanici
        {
            KurumsalKullaniciId = kullanici.KurumsalKullaniciId,
            FirmaEposta = kullanici.FirmaEposta,
            FirmaAdi = kullanici.FirmaAdi,
            FirmaSifre = kullanici.FirmaSifre,
            FirmaSifreKontrol = kullanici.FirmaSifreKontrol,
            FirmaTelefon = kullanici.FirmaTelefon,
            FaturaAdresi = kullanici.FaturaAdresi,
            FirmaIl = kullanici.FirmaIl,
            FirmaIlce = kullanici.FirmaIlce,
            VergiNo = kullanici.VergiNo,
            VergiDairesi = kullanici.VergiDairesi,
            VergiIl = kullanici.VergiIl,
            VergiIlce = kullanici.VergiIlce,
        };

        if (changePassword)
        {
            // Şifreyi günceller.
            userUpdate.FirmaSifre = _passwordHasherKurumsal.HashPassword(userUpdate, kullanici.FirmaSifre);
            userUpdate.FirmaSifreKontrol = _passwordHasherKurumsal.HashPassword(userUpdate, kullanici.FirmaSifreKontrol);
        }

        _kurumsalkullaniciRepository.KullaniciDuzenle(userUpdate);
        return RedirectToAction("UserIndex");
    }
    return View(kullanici);
}

public async Task<IActionResult> KurumsalDelete(int? id)
{
    if (id == null)
    {
        return NotFound();
    }
    // Kurumsal kullanıcıyı ID'ye göre getirir.
    var user = await _kurumsalkullaniciRepository.KurumsalKullanicilar.FirstOrDefaultAsync(i => i.KurumsalKullaniciId == id);
    if (user == null)
    {
        return NotFound();
    }
    // Kullanıcı bilgilerini ViewModel'e ekleyip View'a gönderir.
    return View(new KurumsalKullaniciViewModel
    {
        KurumsalKullaniciId = user.KurumsalKullaniciId,
        FirmaEposta = user.FirmaEposta,
        FirmaAdi = user.FirmaAdi,
        FirmaSifre = user.FirmaSifre,
        FirmaSifreKontrol = user.FirmaSifreKontrol,
        FirmaTelefon = user.FirmaTelefon,
        FaturaAdresi = user.FaturaAdresi,
        FirmaIl = user.FirmaIl,
        FirmaIlce = user.FirmaIlce,
        VergiNo = user.VergiNo,
        VergiDairesi = user.VergiDairesi,
        VergiIl = user.VergiIl,
        VergiIlce = user.VergiIlce,
    });
}

[HttpPost]
public async Task<IActionResult> KurumsalDelete(int id)
{
    // Kurumsal kullanıcıyı ID'ye göre getirir ve siler.
    var entityDelete = await _kurumsalkullaniciRepository.KurumsalKullanicilar.FirstOrDefaultAsync(k => k.KurumsalKullaniciId == id);

    if (entityDelete == null)
    {
        return NotFound();
    }
    _kurumsalkullaniciRepository.KullaniciSil(entityDelete);
    return RedirectToAction("UserIndex");
}

public async Task<IActionResult> Rezervasyonlar()
{
    // Rezervasyon durumunu güncelleyerek tamamlanmış rezervasyonları işaretler.
    _rezervasyonRepository.RezervasyonDurumuGuncelle();
    // Rezervasyonları araç, bireysel ve kurumsal kullanıcı bilgileriyle birlikte getirir.
    var rezervasyonlar = await _rezervasyonRepository.Rezervasyonlar
                                                     .Include(r => r.Arac)
                                                     .Include(r => r.BireyselKullanici)
                                                     .Include(r => r.KurumsalKullanici)
                                                     .ToListAsync();
    return View(new RezervasyonViewModel { Rezervasyonlar = rezervasyonlar });
}

public IActionResult RezervasyonEkle()
{
    // Varsayılan teslim ve iade tarihlerini belirler.
    var teslimTarihi = DateTime.Now;
    var iadeTarihi = DateTime.Now.AddDays(1);

    // Tüm şehirleri getirir ve SelectListItem'a dönüştürür.
    var allCities = _sehirRepository.Sehirler
                                    .AsEnumerable()
                                    .Select(s => new SelectListItem
                                    {
                                        Value = s.SehirId.ToString(),
                                        Text = $"{s.IlAdi}, {s.IlceAdi}"
                                    })
                                    .OrderBy(s => s.Text)
                                    .ToList();

    // Bireysel ve kurumsal kullanıcıları getirir ve SelectListItem'a dönüştürür.
    var bireyselKullanicilar = _kullaniciRepository.BireyselKullanicilar
                    .Select(k => new { k.BireyselKullaniciId, k.Ad, k.Soyad })
                    .ToList()
                    .Select(k => new SelectListItem
                    {
                        Value = $"B-{k.BireyselKullaniciId}",
                        Text = $"{k.Ad} {k.Soyad} (Bireysel)"
                    });

    var kurumsalKullanicilar = _kurumsalkullaniciRepository.KurumsalKullanicilar
                    .Select(k => new { k.KurumsalKullaniciId, k.FirmaAdi })
                    .ToList()
                    .Select(k => new SelectListItem
                    {
                        Value = $"K-{k.KurumsalKullaniciId}",
                        Text = $"{k.FirmaAdi} (Kurumsal)"
                    });

    // Tüm kullanıcıları birleştirip sıralar.
    var allUsers = bireyselKullanicilar.Union(kurumsalKullanicilar)
        .OrderBy(u => u.Text)
        .ToList();

    // Rezervasyon ekleme modelini View'a gönderir.
    var model = new RezervasyonEkleViewModel
    {
        TeslimTarih = teslimTarihi,
        IadeTarih = iadeTarihi,
        TeslimYeriSecenekleri = allCities,
        IadeYeriSecenekleri = allCities,
        KullaniciSecenekleri = allUsers
    };
    return View(model);
}

[HttpPost]
public IActionResult RezervasyonEkle(RezervasyonEkleViewModel model)
{
    if (ModelState.IsValid)
    {
        // Teslim ve iade yeri bilgilerini getirir.
        var teslimYeri = _sehirRepository.Sehirler.FirstOrDefault(s => s.SehirId == model.TeslimYeriId);
        var iadeYeri = _sehirRepository.Sehirler.FirstOrDefault(s => s.SehirId == model.IadeYeriId);

        model.TeslimYeriAdi = teslimYeri?.IlAdi + ", " + teslimYeri?.IlceAdi;
        model.IadeYeriAdi = iadeYeri?.IlAdi + ", " + iadeYeri?.IlceAdi;

        // Seçilen tarihlerde ve lokasyonda müsait olan aracı getirir.
        var selectedCar = _aracRepository.GetAvailableCarsByLocation(model.TeslimTarih, model.IadeTarih, model.TeslimYeriId.GetValueOrDefault())
            .FirstOrDefault(a => a.AracId == model.AracId);

        if (selectedCar == null)
        {
            ModelState.AddModelError("", "Seçilen araç bu tarihler arasında müsait değildir veya bulunamadı.");
            return View(model);
        }

        // Fiyat hesaplaması yapar.
        decimal price = CalculatePrice(model.TeslimTarih, model.IadeTarih, selectedCar.AracFiyat, model.Sigorta);

        // Rezervasyon durumunu kontrol eder.
        if (_rezervasyonRepository.RezervasyonDurumu(selectedCar.AracId, model.TeslimTarih, model.IadeTarih))
        {
            // Yeni rezervasyon oluşturur.
            var yeniRezervasyon = new Rezervasyon
            {
                TeslimTarih = model.TeslimTarih,
                IadeTarih = model.IadeTarih,
                TeslimYeriId = model.TeslimYeriId,
                IadeYeriId = model.IadeYeriId,
                TeslimYeriAdi = model.TeslimYeriAdi,
                IadeYeriAdi = model.IadeYeriAdi,
                Sigorta = model.Sigorta,
                AracId = model.AracId,
                Fiyat = price,
                IsActive = model.IsActive,
                IsCompleted = false,
            };

            // Kullanıcı türüne göre rezervasyon ataması yapar.
            if (model.SelectedKullaniciId.StartsWith("B-"))
            {
                yeniRezervasyon.BireyselKullaniciId = int.Parse(model.SelectedKullaniciId.Substring(2));
            }
            else if (model.SelectedKullaniciId.StartsWith("K-"))
            {
                yeniRezervasyon.KurumsalKullaniciId = int.Parse(model.SelectedKullaniciId.Substring(2));
            }

            _rezervasyonRepository.RezervasyonEkle(yeniRezervasyon);

            return RedirectToAction("Rezervasyonlar");
        }
        else
        {
            ModelState.AddModelError("", "Seçilen tarihler arasında bu araç müsait değildir.");
        }
    }
    return View(model);
}

[HttpGet]
public async Task<IActionResult> RezervasyonDuzenle(int? id)
{
    if (id == null)
    {
        return NotFound();
    }
    // Rezervasyonu ID'ye göre getirir.
    var rezervasyon = await _rezervasyonRepository.Rezervasyonlar.FirstOrDefaultAsync(x => x.RezervasyonId == id);

    if (rezervasyon == null)
    {
        return NotFound();
    }

    // Tüm şehirleri ve müsait araçları getirir.
    var allCities = _sehirRepository.Sehirler
                                    .AsEnumerable()
                                    .Select(s => new SelectListItem
                                    {
                                        Value = s.SehirId.ToString(),
                                        Text = $"{s.IlAdi}, {s.IlceAdi}"
                                    })
                                    .OrderBy(s => s.Text)
                                    .ToList();

    var availableCars = _aracRepository.GetAvailableCars(rezervasyon.TeslimTarih, rezervasyon.IadeTarih)
        .Select(a => new SelectListItem
        {
            Value = a.AracId.ToString(),
            Text = $"{a.Marka} {a.AracModel} - {a.Vites}",
        }).ToList();

    // Rezervasyon düzenleme modelini View'a gönderir.
    var model = new RezervasyonEkleViewModel
    {
        RezervasyonId = rezervasyon.RezervasyonId,
        TeslimTarih = rezervasyon.TeslimTarih,
        TeslimYeriAdi = rezervasyon.TeslimYeriAdi,
        TeslimYeriId = rezervasyon.TeslimYeriId,
        IadeTarih = rezervasyon.IadeTarih,
        IadeYeriId = rezervasyon.IadeYeriId,
        IadeYeriAdi = rezervasyon.IadeYeriAdi,
        AracSecenekleri = availableCars,
        IadeYeriSecenekleri = allCities,
        TeslimYeriSecenekleri = allCities,
        AracId = rezervasyon.AracId,
        Sigorta = rezervasyon.Sigorta,
        IsActive = rezervasyon.IsActive,
        IsCompleted = rezervasyon.IsCompleted,
    };

    return View(model);
}

[HttpPost]
public IActionResult RezervasyonDuzenle(RezervasyonEkleViewModel model)
{
    if (ModelState.IsValid)
    {
        // Rezervasyonu ID'ye göre getirir ve seçilen aracı kontrol eder.
        var rezervasyon = _rezervasyonRepository.Rezervasyonlar.FirstOrDefault(i => i.RezervasyonId == model.RezervasyonId);
        var selectedCar = _aracRepository.Araclar.FirstOrDefault(a => a.AracId == model.AracId);

        if (selectedCar == null)
        {
            ModelState.AddModelError("", "Seçilen araç bu tarihler arasında müsait değildir veya bulunamadı.");
            return View(model);
        }

        // Fiyat hesaplaması yapar.
        decimal price = CalculatePrice(model.TeslimTarih, model.IadeTarih, selectedCar.AracFiyat, model.Sigorta);

        if (rezervasyon != null)
        {
            // Teslim ve iade yeri bilgilerini günceller.
            var teslimYeri = _sehirRepository.Sehirler.FirstOrDefault(s => s.SehirId == model.TeslimYeriId);
            var iadeYeri = _sehirRepository.Sehirler.FirstOrDefault(s => s.SehirId == model.IadeYeriId);

            rezervasyon.TeslimTarih = model.TeslimTarih;
            rezervasyon.IadeTarih = model.IadeTarih;
            rezervasyon.TeslimYeriId = model.TeslimYeriId;
            rezervasyon.TeslimYeriAdi = teslimYeri?.IlAdi + ", " + teslimYeri?.IlceAdi;
            rezervasyon.IadeYeriAdi = iadeYeri?.IlAdi + ", " + iadeYeri?.IlceAdi;
            rezervasyon.IadeYeriId = model.IadeYeriId;
            rezervasyon.Sigorta = model.Sigorta;
            rezervasyon.AracId = model.AracId;
            rezervasyon.Fiyat = price;
            rezervasyon.IsActive = model.IsActive;
            rezervasyon.IsCompleted = model.IsCompleted;

            // Rezervasyonun düzenlenebilirliğini kontrol eder ve günceller.
            if (_rezervasyonRepository.RezervasyonDuzenlemeSorgu(model.RezervasyonId, selectedCar.AracId, model.TeslimTarih, model.IadeTarih))
            {
                try
                {
                    _rezervasyonRepository.RezervasyonDuzenle(rezervasyon);
                    _aracRepository.GuncelleAracKonumu(model.AracId, model.IadeYeriId.GetValueOrDefault());
                    return RedirectToAction("Rezervasyonlar");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu: " + ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "Seçilen tarihler arasında bu araç müsait değildir.");
            }
        }
    }

    // Şehir ve araç seçeneklerini yeniden yükler.
    model.TeslimYeriSecenekleri = _sehirRepository.Sehirler
        .AsEnumerable()
        .Select(s => new SelectListItem
        {
            Value = s.SehirId.ToString(),
            Text = $"{s.IlAdi}, {s.IlceAdi}"
        })
        .OrderBy(s => s.Text)
        .ToList();

    model.IadeYeriSecenekleri = model.TeslimYeriSecenekleri;

    model.AracSecenekleri = _aracRepository.GetAvailableCars(model.TeslimTarih, model.IadeTarih)
        .Select(a => new SelectListItem
        {
            Value = a.AracId.ToString(),
            Text = $"{a.Marka} {a.AracModel} - {a.Vites}",
        }).ToList();

    return View(model);
}

public async Task<IActionResult> RezervasyonSil(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    // Rezervasyonu ID'ye göre getirir ve silme modelini hazırlar.
    var rezervasyon = await _rezervasyonRepository.Rezervasyonlar
        .Include(r => r.Arac)
        .FirstOrDefaultAsync(r => r.RezervasyonId == id);

    if (rezervasyon == null)
    {
        return NotFound();
    }

    var model = new RezervasyonEkleViewModel
    {
        RezervasyonId = rezervasyon.RezervasyonId,
        TeslimTarih = rezervasyon.TeslimTarih,
        IadeTarih = rezervasyon.IadeTarih,
        TeslimYeriAdi = rezervasyon.TeslimYeriAdi,
        IadeYeriAdi = rezervasyon.IadeYeriAdi,
        Sigorta = rezervasyon.Sigorta,
        Arac = rezervasyon.Arac,
        Fiyat = rezervasyon.Fiyat
    };

    return View(model);
}

[HttpPost]
public IActionResult RezervasyonSil(RezervasyonEkleViewModel model)
{
    // Rezervasyonu ID'ye göre getirir ve siler.
    var rezervasyon = _rezervasyonRepository.Rezervasyonlar.FirstOrDefault(r => r.RezervasyonId == model.RezervasyonId);

    if (rezervasyon != null)
    {
        _rezervasyonRepository.RezervasyonSil(rezervasyon);
        return RedirectToAction("Rezervasyonlar");
    }

    return View(model);
}

public async Task<IActionResult> Araclar(string kategori = "Tüm Araçlar")
{
    // Kategoriye göre araçları getirir.
    List<Arac> araclar;

    if (kategori == "Tüm Araçlar")
    {
        araclar = await _aracRepository.Araclar
                                        .Include(r => r.Sehir)
                                        .Include(r => r.Kategori)
                                        .ToListAsync();
    }
    else
    {
        araclar = await _aracRepository.Araclar
                                        .Include(r => r.Sehir)
                                        .Include(r => r.Kategori)
                                        .Where(a => a.Kategori.KategoriAdi == kategori)
                                        .ToListAsync();
    }

    ViewBag.SelectedKategori = kategori;

    return View(new AracViewModel { Araclar = araclar });
}

public IActionResult AracEkle()
{
    // Araç ekleme modelini hazırlar ve View'a gönderir.
    var model = new AracViewModel
    {
        Kategoriler = _kategoriRepository.Kategoriler.ToList(),
        Sehirler = _sehirRepository.Sehirler.ToList()
    };

    return View(model);
}

[HttpPost]
public async Task<IActionResult> AracEkle(AracViewModel model, IFormFile Image)
{
    if (ModelState.IsValid)
    {
        // Görsel dosyası varsa yükler.
        if (model.Image != null && model.Image.Length > 0)
        {
            var resimAd = Path.GetFileNameWithoutExtension(model.Image.FileName);
            var uzanti = Path.GetExtension(model.Image.FileName);
            var yeniResimAd = resimAd + "_" + Guid.NewGuid() + uzanti;

            var kaydetmeYolu = Path.GetFullPath(Path.Combine(_env.WebRootPath, "img", yeniResimAd));

            using (var fileStream = new FileStream(kaydetmeYolu, FileMode.Create))
            {
                await model.Image.CopyToAsync(fileStream);
            }

            model.Resim = yeniResimAd;
        }

        // Yeni araç oluşturur ve kaydeder.
        _aracRepository.AracEkle(
            new Arac
            {
                Marka = model.Marka,
                AracModel = model.AracModel,
                Vites = model.Vites,
                Motor = model.Motor,
                Koltuk = model.Koltuk,
                Renk = model.Renk,
                Resim = model.Resim,
                Plaka = model.Plaka,
                Km = model.Km,
                Musaitlik = model.Musaitlik,
                AracFiyat = model.AracFiyat,
                KategoriId = model.KategoriId,
                SehirId = model.SehirId,
            }
        );
        return RedirectToAction("Index");
    }
    return View(model);
}

public async Task<IActionResult> AracDuzenle(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    // Aracı ID'ye göre getirir.
    var arac = await _aracRepository.Araclar.FirstOrDefaultAsync(i => i.AracId == id);
    if (arac == null)
    {
        return NotFound();
    }

    // Araç düzenleme modelini hazırlar ve View'a gönderir.
    var model = new AracViewModel
    {
        AracId = arac.AracId,
        Marka = arac.Marka,
        AracModel = arac.AracModel,
        Vites = arac.Vites,
        Motor = arac.Motor,
        Koltuk = arac.Koltuk,
        Renk = arac.Renk,
        Resim = arac.Resim,
        Plaka = arac.Plaka,
        Km = arac.Km,
        Musaitlik = arac.Musaitlik,
        AracFiyat = arac.AracFiyat,
        KategoriId = arac.KategoriId,
        SehirId = arac.SehirId,
        Kategoriler = _kategoriRepository.Kategoriler.ToList(),
        Sehirler = _sehirRepository.Sehirler.ToList()
    };

    return View(model);
}

[HttpPost]
public async Task<IActionResult> AracDuzenle(AracViewModel model, IFormFile Image)
{
    if (model.Image == null || model.Image.Length == 0)
    {
        // Görsel dosyası yoksa mevcut resmi korur.
        var currentAracResim = _aracRepository.Araclar.FirstOrDefault(i => i.AracId == model.AracId)?.Resim;
        model.Resim = currentAracResim;
        ModelState.Remove("Image");
    }
    else
    {
        // Görsel dosyası varsa yeni resmi yükler.
        var resimAd = Path.GetFileNameWithoutExtension(model.Image.FileName);
        var uzanti = Path.GetExtension(model.Image.FileName);
        var yeniResimAd = resimAd + "_" + Guid.NewGuid() + uzanti;

        var kaydetmeYolu = Path.GetFullPath(Path.Combine(_env.WebRootPath, "img", yeniResimAd));
        using (var fileStream = new FileStream(kaydetmeYolu, FileMode.Create))
        {
            await model.Image.CopyToAsync(fileStream);
        }

        model.Resim = yeniResimAd;
    }

    if (ModelState.IsValid)
    {
        // Aracı günceller ve kaydeder.
        var entityUpdate = new Arac
        {
            AracId = model.AracId,
            Marka = model.Marka,
            AracModel = model.AracModel,
            Vites = model.Vites,
            Motor = model.Motor,
            Koltuk = model.Koltuk,
            Renk = model.Renk,
            Resim = model.Resim,
            Plaka = model.Plaka,
            Km = model.Km,
            Musaitlik = model.Musaitlik,
            AracFiyat = model.AracFiyat,
            KategoriId = model.KategoriId,
            SehirId = model.SehirId,
        };
        _aracRepository.AracDuzenle(entityUpdate);
        return RedirectToAction("Araclar");
    }
    return View(model);
}

public IActionResult AracSil(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    // Aracı ID'ye göre getirir.
    var arac = _aracRepository.Araclar.FirstOrDefault(i => i.AracId == id);
    if (arac == null)
    {
        return NotFound();
    }

    // Araç bilgilerini silme modeline ekleyip View'a gönderir.
    var model = new AracViewModel
    {
        AracId = arac.AracId,
        Marka = arac.Marka,
        AracModel = arac.AracModel,
        Vites = arac.Vites,
        Motor = arac.Motor,
        Koltuk = arac.Koltuk,
        Renk = arac.Renk,
        Resim = arac.Resim,
        Plaka = arac.Plaka,
        Km = arac.Km,
        Musaitlik = arac.Musaitlik,
        AracFiyat = arac.AracFiyat,
        Kategori = arac.Kategori,
        Sehir = arac.Sehir
    };

    return View(model);
}

[HttpPost]
public IActionResult AracSil(int id)
{
    // Aracı ID'ye göre getirir ve siler.
    var entityDelete = _aracRepository.Araclar.FirstOrDefault(i => i.AracId == id);
    if (entityDelete == null)
    {
        return NotFound();
    }
    _aracRepository.AracSil(entityDelete);
    return RedirectToAction("Index");
}

[HttpGet]
public async Task<IActionResult> GetFilteredCarsJson(string kategori = "Tüm Araçlar")
{
    // Kategoriye göre filtrelenmiş araçları getirir.
    List<Arac> araclar;

    if (kategori == "Tüm Araçlar")
    {
        araclar = await _aracRepository.Araclar
                                        .Include(r => r.Sehir)
                                        .Include(r => r.Kategori)
                                        .ToListAsync();
    }
    else
    {
        araclar = await _aracRepository.Araclar
                                        .Include(r => r.Sehir)
                                        .Include(r => r.Kategori)
                                        .Where(a => a.Kategori.KategoriAdi == kategori)
                                        .ToListAsync();
    }

    // Araçları JSON formatında döner.
    var result = araclar.Select(a => new
    {
        a.AracId,
        a.Marka,
        a.AracModel,
        a.Motor,
        a.Vites,
        a.Koltuk,
        a.Renk,
        a.Km,
        a.Resim,
        a.AracFiyat,
        a.Musaitlik,
        Sehir = a.Sehir != null ? $"{a.Sehir.IlAdi}, {a.Sehir.IlceAdi}" : "Bilinmiyor",
        Kategori = a.Kategori != null ? a.Kategori.KategoriAdi : "Bilinmiyor"
    });

    return Json(result);
}

[HttpGet]
public async Task<IActionResult> GetFilteredReservationsJson(string filter = "All")
{
    // Filtreye göre rezervasyonları getirir.
    List<Rezervasyon> rezervasyonlar = new List<Rezervasyon>();

    if (filter == "All")
    {
        rezervasyonlar = await _rezervasyonRepository.Rezervasyonlar
                                                     .Include(r => r.Arac)
                                                     .Include(r => r.BireyselKullanici)
                                                     .Include(r => r.KurumsalKullanici)
                                                     .ToListAsync();
    }
    else if (filter == "Active")
    {
        rezervasyonlar = await _rezervasyonRepository.Rezervasyonlar
                                                     .Include(r => r.Arac)
                                                     .Include(r => r.BireyselKullanici)
                                                     .Include(r => r.KurumsalKullanici)
                                                     .Where(r => r.IsActive && !r.IsCompleted)
                                                     .ToListAsync();
    }
    else if (filter == "Inactive")
    {
        rezervasyonlar = await _rezervasyonRepository.Rezervasyonlar
                                                     .Include(r => r.Arac)
                                                     .Include(r => r.BireyselKullanici)
                                                     .Include(r => r.KurumsalKullanici)
                                                     .Where(r => !r.IsActive && !r.IsCompleted)
                                                     .ToListAsync();
    }
    else if (filter == "Completed")
    {
        rezervasyonlar = await _rezervasyonRepository.Rezervasyonlar
                                                     .Include(r => r.Arac)
                                                     .Include(r => r.BireyselKullanici)
                                                     .Include(r => r.KurumsalKullanici)
                                                     .Where(r => r.IsCompleted)
                                                     .ToListAsync();
    }
    else if (filter == "Ongoing")
    {
        rezervasyonlar = await _rezervasyonRepository.Rezervasyonlar
                                                     .Include(r => r.Arac)
                                                     .Include(r => r.BireyselKullanici)
                                                     .Include(r => r.KurumsalKullanici)
                                                     .Where(r => !r.IsCompleted)
                                                     .ToListAsync();
    }

    // Rezervasyonları JSON formatında döner.
    var result = rezervasyonlar.Select(r => new
    {
        r.RezervasyonId,
        r.Arac?.Marka,
        r.Arac?.AracModel,
        r.TeslimTarih,
        r.IadeTarih,
        r.TeslimYeriAdi,
        r.IadeYeriAdi,
        r.Sigorta,
        r.Fiyat,
        r.IsActive,
        r.IsCompleted,
        Resim = r.Arac?.Resim,
        KullaniciAdi = r.BireyselKullanici != null
            ? $"{r.BireyselKullanici.Ad} {r.BireyselKullanici.Soyad}"
            : r.KurumsalKullanici?.FirmaAdi
    });

    return Json(result);
}

public IActionResult GetAvailableCars(int rezervasyonId, DateTime teslimTarihi, DateTime iadeTarihi, int teslimYeriId)
{
    // Seçilen tarihlerde ve lokasyonda müsait olan araçları getirir.
    var availableCars = _aracRepository.GetAvailableCarsByLocation(teslimTarihi, iadeTarihi, teslimYeriId)
        .Select(a => new SelectListItem
        {
            Value = a.AracId.ToString(),
            Text = $"{a.Marka} {a.AracModel} - {a.Vites}",
        }).ToList();

    // Mevcut rezervasyon aracını listeye ekler.
    var currentReservation = _rezervasyonRepository.Rezervasyonlar
        .FirstOrDefault(r => r.RezervasyonId == rezervasyonId);
    if (currentReservation != null)
    {
        var currentCar = _aracRepository.Araclar.FirstOrDefault(a => a.AracId == currentReservation.AracId);
        if (currentCar != null && currentReservation.TeslimYeriId == teslimYeriId)
        {
            availableCars.Add(new SelectListItem
            {
                Value = currentCar.AracId.ToString(),
                Text = $"{currentCar.Marka} {currentCar.AracModel} - {currentCar.Vites}",
                Selected = true
            });
        }
    }

    return Json(availableCars);
}

private decimal CalculatePrice(DateTime teslimTarih, DateTime iadeTarih, decimal dailyRate, bool insuranceIncluded)
{
    // Fiyat hesaplaması yapar.
    int rentalDays = (iadeTarih - teslimTarih).Days;
    decimal basePrice = rentalDays * dailyRate;

    if (insuranceIncluded)
    {
        decimal insuranceRate = basePrice * 0.10m; // Sigorta ücreti toplam fiyatın %10'u
        basePrice += insuranceRate;
    }

    return basePrice;
}
    }
}