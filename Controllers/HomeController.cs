using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rent_A_Car.Data.Abstract;
using Rent_A_Car.Data.Concrete.EfCore;
using Rent_A_Car.Models;
using System.Diagnostics;

namespace Rent_A_Car.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; // Logger bağımlılığı
        private readonly IRezervasyonRepository _rezervasyonRepository; // Rezervasyon veritabanı işlemleri için repository
        private readonly ISehirRepository _sehirRepository; // Şehir veritabanı işlemleri için repository
        private readonly IKategoriRepository _kategoriRepository; // Kategori veritabanı işlemleri için repository
        private readonly IAracRepository _aracRepository; // Araç veritabanı işlemleri için repository

        // Constructor, bağımlılıkları enjekte eder
        public HomeController(ILogger<HomeController> logger, IRezervasyonRepository rezervasyonRepository, IAracRepository aracRepository, ISehirRepository sehirRepository, IKategoriRepository kategoriRepository)
        {
            _logger = logger;
            _rezervasyonRepository = rezervasyonRepository;
            _sehirRepository = sehirRepository;
            _kategoriRepository = kategoriRepository;
            _aracRepository = aracRepository;
        }

        // Ana sayfa görüntüleme
        public IActionResult Index()
        {
            // Teslim yeri seçeneklerini alır
            var teslimYeriSecenekleri = _sehirRepository.Sehirler.Select(s => new SelectListItem
            {
                Value = s.SehirId.ToString(),
                Text = s.IlAdi + ", " + s.IlceAdi
            }).ToList();

            // İade yeri seçeneklerini alır
            var iadeYeriSecenekleri = _sehirRepository.Sehirler.Select(s => new SelectListItem
            {
                Value = s.SehirId.ToString(),
                Text = s.IlAdi + ", " + s.IlceAdi
            }).ToList();

            // Kategori seçeneklerini alır
            var kategoriSecenekleri = _kategoriRepository.Kategoriler.Select(k => new SelectListItem
            {
                Value = k.KategoriId.ToString(),
                Text = k.KategoriAdi
            }).ToList();

            // "Tümü" seçeneğini ekler
            kategoriSecenekleri.Insert(0, new SelectListItem { Value = "0", Text = "Tümü" });

            // Rezervasyon arama modelini oluşturur
            var model = new RezervasyonAramaViewModel
            {
                TeslimTarih = DateTime.Now, // Teslim tarihini bugünün tarihi olarak ayarlar
                IadeTarih = DateTime.Now.AddDays(1), // İade tarihini bir gün sonrasına ayarlar
                TeslimYeriSecenekleri = teslimYeriSecenekleri, // Teslim yeri seçeneklerini ekler
                IadeYeriSecenekleri = iadeYeriSecenekleri, // İade yeri seçeneklerini ekler
                KategoriSecenekleri = kategoriSecenekleri // Kategori seçeneklerini ekler
            };

            return View(model); // Modeli View'a gönderir
        }

        // Araç arama işlemi
        [HttpPost]
        public IActionResult Search(RezervasyonAramaViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Seçilen teslim tarihi, iade tarihi ve teslim yerine göre müsait araçları getirir
                var availableCars = _aracRepository.GetAvailableCarsByLocation(model.TeslimTarih, model.IadeTarih, model.TeslimYeriId);

                // Eğer bir kategori seçilmişse, araçları kategoriye göre filtreler
                if (model.KategoriId != 0)
                {
                    availableCars = availableCars.Where(a => a.KategoriId == model.KategoriId);
                }

                // Müsait araçları modele ekler
                model.Araclar = availableCars.ToList();

                // Teslim ve iade yeri seçeneklerini yeniden yükler
                model.TeslimYeriSecenekleri = _sehirRepository.Sehirler.Select(s => new SelectListItem
                {
                    Value = s.SehirId.ToString(),
                    Text = s.IlAdi + ", " + s.IlceAdi
                }).ToList();
                model.IadeYeriSecenekleri = model.TeslimYeriSecenekleri; // Teslim yeri seçeneklerini iade yeri seçeneklerine kopyalar

                return View("SearchResults", model); // Sonuçları SearchResults View'ında gösterir
            }
            return View("Index", model); // Model geçerli değilse, aynı View'ı yeniden döner
        }
    }
}
