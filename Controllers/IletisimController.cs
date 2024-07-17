using Microsoft.AspNetCore.Mvc;
using Rent_A_Car.Data.Abstract;
using Rent_A_Car.Entity;
using Rent_A_Car.Models;

namespace Rent_A_Car.Controllers
{
    public class IletisimController : Controller
    {
        private readonly IIletisimRepository _iletisimRepository; // İletişim veritabanı işlemleri için repository

        // Constructor, bağımlılığı enjekte eder
        public IletisimController(IIletisimRepository iletisimRepository)
        {
            _iletisimRepository = iletisimRepository;
        }

        // İletişim formunu görüntüler
        [HttpGet]
        public IActionResult Index()
        {
            return View(new IletisimViewModel()); // Boş bir iletişim modeli ile View'ı döner
        }

        // İletişim formunu gönderme işlemi
        [HttpPost]
        public IActionResult Index(IletisimViewModel model)
        {
            // Model doğrulamasını kontrol eder
            if (ModelState.IsValid)
            {
                // Yeni iletişim mesajı oluşturur
                var iletisim = new Iletisim
                {
                    IletisimAdSoyad = model.IletisimAdSoyad, // Kullanıcının ad ve soyadı
                    IletisimEposta = model.IletisimEposta, // Kullanıcının e-posta adresi
                    Telefon = model.Telefon, // Kullanıcının telefon numarası
                    Mesaj = model.Mesaj, // Kullanıcının mesajı
                    IsRead = false, // Mesajın okunmadığını belirtir
                    SentDate = DateTime.Now // Mesajın gönderilme tarihini ayarlar
                };

                // Mesajı veritabanına ekler
                _iletisimRepository.MesajEkle(iletisim);

                // Başarı mesajını ViewBag'e ekler
                ViewBag.Message = "Mesajınız başarıyla gönderildi.";

                // Formu temizlemek için ModelState'i temizler
                ModelState.Clear();

                // Boş bir iletişim modeli ile View'ı yeniden döner
                return View(new IletisimViewModel());
            }

            // Model geçerli değilse, aynı View'ı mevcut model ile döner
            return View(model);
        }
    }
}
