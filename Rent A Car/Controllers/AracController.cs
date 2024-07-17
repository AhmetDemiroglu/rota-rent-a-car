using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using Rent_A_Car.Data.Abstract;
using Rent_A_Car.Data.Concrete.EfCore;
using Rent_A_Car.Entity;
using Rent_A_Car.Models;
using System.Numerics;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Rent_A_Car.Controllers
{
    public class AracController : Controller
    {
        private readonly IAracRepository _aracRepository;  // Araç veritabanı işlemleri için repository
        private IWebHostEnvironment _env;  // Web ortamını temsil eder, dosya yolları için kullanılır
        private readonly IKategoriRepository _kategoriRepository;  // Kategori veritabanı işlemleri için repository
        private readonly ISehirRepository _sehirRepository;  // Şehir veritabanı işlemleri için repository
        private readonly IRezervasyonRepository _rezervasyonRepository;  // Rezervasyon veritabanı işlemleri için repository

        // Constructor, bağımlılıkları enjekte eder
        public AracController(IAracRepository aracRepository, IWebHostEnvironment env, IKategoriRepository kategoriRepository, ISehirRepository sehirRepository, IRezervasyonRepository rezervasyonRepository)
        {
            _aracRepository = aracRepository;
            _sehirRepository = sehirRepository;
            _kategoriRepository = kategoriRepository;
            _env = env;
            _rezervasyonRepository = rezervasyonRepository;
        }

        // Araç listesini görüntüler, kategoriye göre filtreler
        public async Task<IActionResult> Index(string kategori = "Tüm Araçlar")
        {
            List<Arac> araclar;

            if (kategori == "Tüm Araçlar")
            {
                araclar = await _aracRepository.Araclar
                                                .Include(r => r.Sehir)  // Şehir bilgilerini ekler
                                                .Include(r => r.Kategori)  // Kategori bilgilerini ekler
                                                .ToListAsync();  // Asenkron olarak listele
            }
            else
            {
                araclar = await _aracRepository.Araclar
                                                .Include(r => r.Sehir)  // Şehir bilgilerini ekler
                                                .Include(r => r.Kategori)  // Kategori bilgilerini ekler
                                                .Where(a => a.Kategori.KategoriAdi == kategori)  // Kategoriye göre filtreler
                                                .ToListAsync();  // Asenkron olarak listele
            }

            ViewBag.SelectedKategori = kategori;  // Seçili kategoriyi ViewBag'e ekler

            return View(new AracViewModel { Araclar = araclar });  // Araçları ViewModel'e ekleyerek View'a döner
        }

        // Yeni araç oluşturma formunu görüntüler
        [Authorize]
        public IActionResult Create(AracViewModel model)
        {
            // Kategorileri listeye ekler
            List<SelectListItem> kategoriValues = (from x in _kategoriRepository.Kategoriler.ToList()
                                                   select new SelectListItem
                                                   {
                                                       Text = x.KategoriAdi,
                                                       Value = x.KategoriId.ToString()
                                                   }).ToList();
            ViewBag.kv = kategoriValues;

            // Şehirleri listeye ekler
            List<SelectListItem> sehirValues = (from x in _sehirRepository.Sehirler.ToList()
                                                select new SelectListItem
                                                {
                                                    Value = x.SehirId.ToString(),
                                                    Text = $"{x.IlAdi}, {x.IlceAdi}"
                                                }).ToList();
            ViewBag.sv = sehirValues;

            return View();  // View'ı döner
        }

        // Yeni araç oluşturma işlemini gerçekleştirir
        [HttpPost, Authorize]
        public async Task<IActionResult> Create(AracViewModel model, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                if (model.Image != null && model.Image.Length > 0)
                {
                    var resimAd = Path.GetFileNameWithoutExtension(model.Image.FileName);  // Dosya adını uzantısız alır
                    var uzanti = Path.GetExtension(model.Image.FileName);  // Dosya uzantısını alır
                    var yeniResimAd = resimAd + "_" + Guid.NewGuid() + uzanti;  // Benzersiz yeni dosya adı oluşturur

                    var kaydetmeYolu = Path.GetFullPath(Path.Combine(_env.WebRootPath, "img", yeniResimAd));  // Dosyanın kaydedileceği tam yolu oluşturur

                    using (var fileStream = new FileStream(kaydetmeYolu, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);  // Dosyayı asenkron olarak kaydeder
                    }

                    model.Resim = yeniResimAd;  // Modeldeki Resim alanını günceller
                }

                // Yeni araç oluşturur ve veritabanına ekler
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
                return RedirectToAction("Index");  // Index sayfasına yönlendirir
            }
            return View();  // Model geçerli değilse, aynı View'ı yeniden döner
        }

        // Araç düzenleme formunu görüntüler
        public async Task<IActionResult> Edit(int? id)
        {
            // Kategorileri listeye ekler
            List<SelectListItem> kategoriValues = (from x in _kategoriRepository.Kategoriler.ToList()
                                                   select new SelectListItem
                                                   {
                                                       Text = x.KategoriAdi,
                                                       Value = x.KategoriId.ToString()
                                                   }).ToList();
            ViewBag.kv = kategoriValues;

            // Şehirleri listeye ekler
            List<SelectListItem> sehirValues = (from x in _sehirRepository.Sehirler.ToList()
                                                select new SelectListItem
                                                {
                                                    Value = x.SehirId.ToString(),
                                                    Text = $"{x.IlAdi}, {x.IlceAdi}"
                                                }).ToList();
            ViewBag.sv = sehirValues;

            if (id == null)
            {
                return NotFound();  // ID null ise NotFound döner
            }

            var arac = await _aracRepository.Araclar.FirstOrDefaultAsync(i => i.AracId == id);  // ID'ye göre aracı bulur
            if (arac == null)
            {
                return NotFound();  // Araç bulunamazsa NotFound döner
            }

            return View(new AracViewModel
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
                KategoriId = arac.KategoriId,
                SehirId = arac.SehirId,
            });
        }

        // Araç düzenleme işlemini gerçekleştirir
        [Authorize, HttpPost]
        public async Task<IActionResult> Edit(AracViewModel model, IFormFile Image)
        {
            if (model.Image == null || model.Image.Length == 0)
            {
                var currentAracResim = _aracRepository.Araclar.FirstOrDefault(i => i.AracId == model.AracId)?.Resim;  // Mevcut resmi korur
                model.Resim = currentAracResim;
                ModelState.Remove("Image");  // Image doğrulamasını kaldırır
            }
            else
            {
                var resimAd = Path.GetFileNameWithoutExtension(model.Image.FileName);  // Dosya adını uzantısız alır
                var uzanti = Path.GetExtension(model.Image.FileName);  // Dosya uzantısını alır
                var yeniResimAd = resimAd + "_" + Guid.NewGuid() + uzanti;  // Benzersiz yeni dosya adı oluşturur

                var kaydetmeYolu = Path.GetFullPath(Path.Combine(_env.WebRootPath, "img", yeniResimAd));  // Dosyanın kaydedileceği tam yolu oluşturur
                using (var fileStream = new FileStream(kaydetmeYolu, FileMode.Create))
                {
                    await model.Image.CopyToAsync(fileStream);  // Dosyayı asenkron olarak kaydeder
                }

                model.Resim = yeniResimAd;  // Modeldeki Resim alanını günceller
            }

            if (ModelState.IsValid)
            {
                // Aracı günceller ve veritabanına kaydeder
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
                    KategoriId = model.KategoriId,
                    SehirId = model.SehirId,
                    AracFiyat = model.AracFiyat,
                };
                _aracRepository.AracDuzenle(entityUpdate);  // Aracı günceller
                return RedirectToAction("Index");  // Index sayfasına yönlendirir
            }
            return View(model);  // Model geçerli değilse, aynı View'ı yeniden döner
        }

        // Araç silme formunu görüntüler
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();  // ID null ise NotFound döner
            }

            var arac = _aracRepository.Araclar.FirstOrDefault(i => i.AracId == id);  // ID'ye göre aracı bulur
            if (arac == null)
            {
                return NotFound();  // Araç bulunamazsa NotFound döner
            }

            return View(new AracViewModel
            {
                AracId = arac.AracId,
                AracModel = arac.AracModel,
                Kategori = arac.Kategori,
                AracFiyat = arac.AracFiyat,
                KategoriId = arac.KategoriId,
                Km = arac.Km,
                Koltuk = arac.Koltuk,
                Marka = arac.Marka,
                Motor = arac.Motor,
                Musaitlik = arac.Musaitlik,
                Plaka = arac.Plaka,
                Resim = arac.Resim,
                Renk = arac.Renk,
                Sehir = arac.Sehir,
                SehirId = arac.SehirId,
                Vites = arac.Vites,
            });
        }

        // Araç silme işlemini gerçekleştirir
        [Authorize, HttpPost]
        public IActionResult Delete(int id)
        {
            var entityDelete = _aracRepository.Araclar.FirstOrDefault(i => i.AracId == id);  // ID'ye göre aracı bulur
            if (entityDelete == null)
            {
                return NotFound();  // Araç bulunamazsa NotFound döner
            }
            _aracRepository.AracSil(entityDelete);  // Aracı siler
            return RedirectToAction("Index");  // Index sayfasına yönlendirir
        }

        // Filtrelenmiş araç listesini JSON formatında döner
        [HttpGet]
        public async Task<IActionResult> GetFilteredCarsJson(string kategori = "Tüm Araçlar")
        {
            List<Arac> araclar;

            if (kategori == "Tüm Araçlar")
            {
                araclar = await _aracRepository.Araclar
                                                .Include(r => r.Sehir)  // Şehir bilgilerini ekler
                                                .Include(r => r.Kategori)  // Kategori bilgilerini ekler
                                                .ToListAsync();  // Asenkron olarak listele
            }
            else
            {
                araclar = await _aracRepository.Araclar
                                                .Include(r => r.Sehir)  // Şehir bilgilerini ekler
                                                .Include(r => r.Kategori)  // Kategori bilgilerini ekler
                                                .Where(a => a.Kategori.KategoriAdi == kategori)  // Kategoriye göre filtreler
                                                .ToListAsync();  // Asenkron olarak listele
            }

            // Araç listesini JSON formatına dönüştürerek döner
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
                Sehir = a.Sehir != null ? $"{a.Sehir.IlAdi}, {a.Sehir.IlceAdi}" : "Bilinmiyor",
                Kategori = a.Kategori != null ? a.Kategori.KategoriAdi : "Bilinmiyor"
            });

            return Json(result);  // JSON formatında döner
        }
    }
}
