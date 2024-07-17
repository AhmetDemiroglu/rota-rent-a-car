using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rent_A_Car.Data.Abstract;
using Rent_A_Car.Entity;
using Rent_A_Car.Models;
using System.Security.Claims;

namespace Rent_A_Car.Controllers
{
    public class RezervasyonController : Controller
    {
        private readonly IRezervasyonRepository _rezervasyonRepository;
        private readonly IAracRepository _aracRepository;
        private readonly ISehirRepository _sehirRepository;
        private readonly IBireyselKullaniciRepository _bireyselKullaniciRepository;
        private readonly IKurumsalKullaniciRepository _kurumsalKullaniciRepository;
        private readonly IKategoriRepository _kategoriRepository;

        // Constructor: Bağımlılıkları alır.
        public RezervasyonController(IRezervasyonRepository rezervasyonRepository, IAracRepository aracRepository, ISehirRepository sehirRepository, IBireyselKullaniciRepository bireyselKullaniciRepository, IKurumsalKullaniciRepository kurumsalKullaniciRepository, IKategoriRepository kategoriRepository)
        {
            _rezervasyonRepository = rezervasyonRepository;
            _aracRepository = aracRepository;
            _sehirRepository = sehirRepository;
            _bireyselKullaniciRepository = bireyselKullaniciRepository;
            _kurumsalKullaniciRepository = kurumsalKullaniciRepository;
            _kategoriRepository = kategoriRepository;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Rezervasyon durumunu güncelleyerek tamamlanmış rezervasyonları işaretler.
            _rezervasyonRepository.RezervasyonDurumuGuncelle();

            // Kullanıcının kimlik bilgisinden kullanıcı ID'sini alır.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kullanıcının tipi (Bireysel veya Kurumsal)
            var kullaniciTipi = User.Claims.FirstOrDefault(x => x.Type == "KullaniciTipi")?.Value;

            // Tüm rezervasyonları alır.
            IQueryable<Rezervasyon> rezervasyonlar = _rezervasyonRepository.Rezervasyonlar;

            // Admin kullanıcı ise tüm rezervasyonları ve araç bilgilerini alır.
            if (User.IsInRole("admin"))
            {
                rezervasyonlar = rezervasyonlar.Include(r => r.Arac);
            }
            else
            {
                // Admin olmayan kullanıcı için, sadece kullanıcıya ait bireysel veya kurumsal rezervasyonları ve araç bilgilerini alır.
                if (kullaniciTipi == "Bireysel")
                {
                    rezervasyonlar = rezervasyonlar
                                    .Where(r => r.BireyselKullaniciId.ToString() == userId)
                                    .Include(r => r.Arac);
                }
                else if (kullaniciTipi == "Kurumsal")
                {
                    rezervasyonlar = rezervasyonlar
                                    .Where(r => r.KurumsalKullaniciId.ToString() == userId)
                                    .Include(r => r.Arac);
                }
            }

            // Asenkron olarak rezervasyon listesini alır.
            var rezervasyonList = await rezervasyonlar.ToListAsync();

            // Her rezervasyon için ilgili bireysel veya kurumsal kullanıcı bilgilerini getirir.
            foreach (var rezervasyon in rezervasyonList)
            {
                // Bireysel kullanıcı bilgilerini getirir.
                if (rezervasyon.BireyselKullaniciId != null)
                {
                    rezervasyon.BireyselKullanici = await _bireyselKullaniciRepository.BireyselKullanicilar
                        .FirstOrDefaultAsync(bk => bk.BireyselKullaniciId == rezervasyon.BireyselKullaniciId);
                }
                // Kurumsal kullanıcı bilgilerini getirir.
                if (rezervasyon.KurumsalKullaniciId != null)
                {
                    rezervasyon.KurumsalKullanici = await _kurumsalKullaniciRepository.KurumsalKullanicilar
                        .FirstOrDefaultAsync(kk => kk.KurumsalKullaniciId == rezervasyon.KurumsalKullaniciId);
                }
            }

            // Rezervasyon listesini bir ViewModel'e ekleyip, bu ViewModel'i View'a gönderir.
            return View(new RezervasyonViewModel { Rezervasyonlar = rezervasyonList });
        }
        public IActionResult Create(int aracId, int teslimYeriId, int iadeYeriId)
        {
            // Seçilen araç bilgilerini getirir.
            var arac = _aracRepository.Araclar.FirstOrDefault(a => a.AracId == aracId);

            // Tüm şehirleri getirir ve SelectListItem olarak düzenler.
            var allCities = _sehirRepository.Sehirler
                                            .AsEnumerable()
                                            .Select(s => new SelectListItem
                                            {
                                                Value = s.SehirId.ToString(),
                                                Text = $"{s.IlAdi}, {s.IlceAdi}"
                                            })
                                            .OrderBy(s => s.Text)
                                            .ToList();

            // Rezervasyon oluşturma modelini hazırlayıp, View'a gönderir.
            var model = new RezervasyonEkleViewModel
            {
                TeslimYeriId = teslimYeriId,
                IadeYeriId = iadeYeriId,
                AracId = aracId,
                TeslimYeriSecenekleri = allCities,
                IadeYeriSecenekleri = allCities,
                Arac = arac
            };

            // Teslim ve iade yeri bilgilerini ekler.
            var teslimYeri = _sehirRepository.Sehirler.FirstOrDefault(s => s.SehirId == teslimYeriId);
            var iadeYeri = _sehirRepository.Sehirler.FirstOrDefault(s => s.SehirId == iadeYeriId);
            model.TeslimYeriAdi = teslimYeri?.IlAdi + ", " + teslimYeri?.IlceAdi;
            model.IadeYeriAdi = iadeYeri?.IlAdi + ", " + iadeYeri?.IlceAdi;

            return View(model);
        }

        [HttpPost, Authorize]
        public IActionResult Create(RezervasyonEkleViewModel model)
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
                    // Kullanıcı kimliğini doğrular.
                    int userId;
                    if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId))
                    {
                        ModelState.AddModelError("", "Kullanıcı kimliği hatalı.");
                        return View(model);
                    }

                    var username = User.FindFirstValue(ClaimTypes.GivenName);
                    var kullanici = _bireyselKullaniciRepository.BireyselKullanicilar.FirstOrDefault(x => x.BireyselKullaniciId == userId)?.Ad;

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
                        IsActive = false,
                        IsCompleted = false,
                    };

                    // Kullanıcı türüne göre rezervasyon ataması yapar.
                    if (kullanici == username)
                    {
                        yeniRezervasyon.BireyselKullaniciId = userId;
                    }
                    else
                    {
                        yeniRezervasyon.KurumsalKullaniciId = userId;
                    }

                    // Rezervasyonu veritabanına ekler ve araç konumunu günceller.
                    _rezervasyonRepository.RezervasyonEkle(yeniRezervasyon);
                    _aracRepository.GuncelleAracKonumu(model.AracId, model.IadeYeriId.GetValueOrDefault());

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Seçilen tarihler arasında bu araç müsait değildir.");
                }
            }
            return View(model);
        }
        [Authorize, HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var rezervasyon = await _rezervasyonRepository.Rezervasyonlar.FirstOrDefaultAsync(x => x.RezervasyonId == id);

            if (rezervasyon == null)
            {
                return NotFound();
            }

            // Tüm şehirleri getirir ve SelectListItem olarak düzenler.
            var allCities = _sehirRepository.Sehirler
                                            .AsEnumerable()
                                            .Select(s => new SelectListItem
                                            {
                                                Value = s.SehirId.ToString(),
                                                Text = $"{s.IlAdi}, {s.IlceAdi}"
                                            })
                                            .OrderBy(s => s.Text)
                                            .ToList();

            // Mevcut tarihlerde ve lokasyonlarda müsait olan araçları getirir.
            var availableCars = _aracRepository.GetAvailableCars(rezervasyon.TeslimTarih, rezervasyon.IadeTarih)
                .Select(a => new SelectListItem
                {
                    Value = a.AracId.ToString(),
                    Text = $"{a.Marka} {a.AracModel} - {a.Vites}",
                }).ToList();

            // Düzenleme için model oluşturur.
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

        [Authorize, HttpPost]
        public IActionResult Edit(RezervasyonEkleViewModel model)
        {
            if (ModelState.IsValid)
            {
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

                    // Rezervasyon güncelleme sorgusunu yapar.
                    if (_rezervasyonRepository.RezervasyonDuzenlemeSorgu(model.RezervasyonId, selectedCar.AracId, model.TeslimTarih, model.IadeTarih))
                    {
                        try
                        {
                            _rezervasyonRepository.RezervasyonDuzenle(rezervasyon);
                            _aracRepository.GuncelleAracKonumu(model.AracId, model.IadeYeriId.GetValueOrDefault());
                            return RedirectToAction("Index");
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

            // Tüm şehirleri getirir ve SelectListItem olarak düzenler.
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

            // Müsait araçları getirir.
            model.AracSecenekleri = _aracRepository.GetAvailableCars(model.TeslimTarih, model.IadeTarih)
                .Select(a => new SelectListItem
                {
                    Value = a.AracId.ToString(),
                    Text = $"{a.Marka} {a.AracModel} - {a.Vites}",
                }).ToList();

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var rezervasyon = await _rezervasyonRepository.Rezervasyonlar.Include(r => r.Arac).FirstOrDefaultAsync(r => r.RezervasyonId == id);
            if (rezervasyon == null)
            {
                return NotFound();
            }

            // Silme için model oluşturur.
            var model = new RezervasyonEkleViewModel
            {
                RezervasyonId = rezervasyon.RezervasyonId,
                TeslimTarih = rezervasyon.TeslimTarih,
                IadeTarih = rezervasyon.IadeTarih,
                TeslimYeriAdi = rezervasyon.TeslimYeriAdi,
                IadeYeriAdi = rezervasyon.IadeYeriAdi,
                Arac = rezervasyon.Arac,
                Sigorta = rezervasyon.Sigorta,
                Fiyat = rezervasyon.Fiyat
            };

            return View(model);
        }

        [Authorize, HttpPost]
        public async Task<IActionResult> Delete(RezervasyonEkleViewModel model)
        {
            // Silinecek rezervasyonu bulur.
            var entityDelete = await _rezervasyonRepository.Rezervasyonlar.FirstOrDefaultAsync(r => r.RezervasyonId == model.RezervasyonId);
            if (entityDelete == null)
            {
                return NotFound();
            }
            _rezervasyonRepository.RezervasyonSil(entityDelete);
            return RedirectToAction("Index");
        }

        // Fiyat hesaplama metodu
        private decimal CalculatePrice(DateTime teslimTarih, DateTime iadeTarih, decimal dailyRate, bool insuranceIncluded)
        {
            int rentalDays = (iadeTarih - teslimTarih).Days;
            decimal basePrice = rentalDays * dailyRate;

            if (insuranceIncluded)
            {
                decimal insuranceRate = basePrice * 0.10m; // Sigorta ücreti toplam fiyatın %10'u
                basePrice += insuranceRate;
            }

            return basePrice;
        }
        // Fiyat hesaplama metodu (JS Ajax çağrıları için)
        [HttpGet]
        public IActionResult CalculatePrice(DateTime teslimTarihi, DateTime iadeTarihi, int aracId, bool sigorta)
        {
            // Veritabanından aracId'ye göre aracı getirir.
            var arac = _aracRepository.Araclar.FirstOrDefault(a => a.AracId == aracId);

            // Eğer araç bulunamazsa, tüm fiyatları "₺0,00" olarak döndürür.
            if (arac == null)
            {
                return Json(new { kiralamaUcreti = "₺0,00", sigortaUcreti = "₺0,00", toplamUcret = "₺0,00" });
            }

            // Temel kiralama ücretini hesaplar.
            decimal basePrice = CalculateBasePrice(teslimTarihi, iadeTarihi, arac.AracFiyat);

            // Eğer sigorta seçildiyse, sigorta ücretini hesaplar (%10 oranında).
            decimal sigortaUcreti = sigorta ? basePrice * 0.10m : 0;

            // Toplam ücreti hesaplar (temel kiralama ücreti + sigorta ücreti).
            decimal toplamUcret = basePrice + sigortaUcreti;

            // Hesaplanan ücretleri JSON formatında döndürür.
            return Json(new
            {
                kiralamaUcreti = basePrice.ToString("C"), // Temel kiralama ücreti ("C" = "Currency") 
                sigortaUcreti = sigortaUcreti.ToString("C"), // Sigorta ücreti
                toplamUcret = toplamUcret.ToString("C") // Toplam ücret
            });
        }

        // Temel kiralama ücretini hesaplar.
        private decimal CalculateBasePrice(DateTime teslimTarih, DateTime iadeTarih, decimal dailyRate)
        {
            // Kiralama gün sayısını hesaplar.
            int rentalDays = (iadeTarih - teslimTarih).Days;

            // Günlük ücret ile kiralama gün sayısını çarparak toplam ücreti hesaplar.
            return rentalDays * dailyRate;
        }

        [HttpGet]
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

        [HttpGet]
        public IActionResult GetCitiesForCar(int aracId)
        {
            // Belirtilen araç için şehirleri getirir.
            var cities = _aracRepository.GetCitiesByCarId(aracId)
                .Select(s => new SelectListItem
                {
                    Value = s.SehirId.ToString(),
                    Text = $"{s.IlAdi}, {s.IlceAdi}"
                }).ToList();

            return Json(cities);
        }
    }
}
