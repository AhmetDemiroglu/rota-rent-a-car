using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rent_A_Car.Data.Abstract;
using Rent_A_Car.Entity;
using Rent_A_Car.Models;
using System.Security.Claims;

namespace Rent_A_Car.Controllers
{
    public class BireyselKullaniciController : Controller
    {
        private readonly IBireyselKullaniciRepository _kullaniciRepository; // Bireysel kullanıcı işlemleri için repository
        private readonly IPasswordHasher<BireyselKullanici> _passwordHasher; // Parola hashleme için

        public BireyselKullaniciController(IBireyselKullaniciRepository kullaniciRepository, IRezervasyonRepository rezervasyonRepository, IAracRepository aracRepository, IPasswordHasher<BireyselKullanici> passwordHasher)
        {
            _kullaniciRepository = kullaniciRepository;
            _passwordHasher = passwordHasher;
        }

        // Kayıt sayfasını görüntüler
        public IActionResult Register()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home"); // Giriş yapmış kullanıcıyı ana sayfaya yönlendirir
            }
            return View(); // Kayıt formunu gösterir
        }

        // Kayıt işlemi
        [HttpPost]
        public IActionResult Register(BireyselKullaniciViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_kullaniciRepository.KullaniciDurumu(model.Eposta!))
                {
                    ModelState.AddModelError("", "Bu e-posta zaten kullanılıyor."); // E-posta adresi zaten kayıtlıysa hata mesajı ekler
                    return View(model);
                }

                // Yeni kullanıcı oluşturur
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

                entity.Sifre = _passwordHasher.HashPassword(entity, model.Sifre); // Parolayı hashler
                entity.SifreKontrol = entity.Sifre; // Parola kontrolü için hashlenmiş parolayı ekler

                _kullaniciRepository.KullaniciKayit(entity); // Yeni kullanıcıyı veritabanına ekler
                return RedirectToAction("Login"); // Başarılı kayıt sonrası giriş sayfasına yönlendirir
            }
            return View(model); // Model geçersizse kayıt formunu yeniden gösterir
        }

        // Giriş sayfasını görüntüler
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home"); // Giriş yapmış kullanıcıyı ana sayfaya yönlendirir
            }
            return View(); // Giriş formunu gösterir
        }

        // Giriş işlemi
        [HttpPost]
        public async Task<IActionResult> Login(BireyselLoginViewModel kullanici)
        {
            if (ModelState.IsValid)
            {
                var isUser = _kullaniciRepository.BireyselKullanicilar.FirstOrDefault(x => x.Eposta == kullanici.Eposta); // Kullanıcıyı e-posta ile bulur
                if (isUser != null && _passwordHasher.VerifyHashedPassword(isUser, isUser.Sifre, kullanici.Sifre) == PasswordVerificationResult.Success)
                {
                    // Kullanıcı giriş bilgilerini ayarlar
                    var userClaims = new List<Claim>
                    {
                        new (ClaimTypes.NameIdentifier, isUser.BireyselKullaniciId.ToString()), // Kullanıcı ID'si
                        new (ClaimTypes.GivenName, isUser.Ad ?? ""), // Kullanıcı adı
                        new (ClaimTypes.Surname, isUser.Soyad ?? ""), // Kullanıcı soyadı
                        new ("KullaniciTipi", "Bireysel") // Kullanıcı tipi
                    };

                    if (isUser.Eposta == "ahmetdemiroglu@gmail.com")
                    {
                        userClaims.Add(new Claim(ClaimTypes.Role, "admin")); // Belirli bir e-posta adresi ile giriş yapan kullanıcıya admin rolü ekler
                    }

                    var userIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Kalıcı oturum açma
                    };
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Mevcut oturumu sonlandırır
                    await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(userIdentity),
                                authProperties
                                ); // Yeni oturumu başlatır
                    return RedirectToAction("Index", "Home"); // Giriş sonrası ana sayfaya yönlendirir
                }
            }
            else
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı."); // Geçersiz giriş bilgileri için hata mesajı ekler
            }
            return View();
        }

        // Çıkış işlemi
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Oturumu sonlandırır
            return RedirectToAction("Login"); // Çıkış sonrası giriş sayfasına yönlendirir
        }

        // Kullanıcı düzenleme sayfasını görüntüler
        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Geçersiz ID için 404 hatası döner
            }
            var kullanici = _kullaniciRepository.BireyselKullanicilar.FirstOrDefault(i => i.BireyselKullaniciId == id); // Kullanıcıyı ID ile bulur
            if (kullanici == null)
            {
                return NotFound(); // Kullanıcı bulunamazsa 404 hatası döner
            }
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
            }); // Kullanıcı bilgilerini view model ile döner
        }

        // Kullanıcı düzenleme işlemi
        [Authorize, HttpPost]
        public async Task<IActionResult> Edit(BireyselKullaniciViewModel kullanici, bool changePassword)
        {
            if (!changePassword)
            {
                ModelState.Remove("Sifre");
                ModelState.Remove("SifreKontrol");
                var currentUser = _kullaniciRepository.BireyselKullanicilar.FirstOrDefault(u => u.BireyselKullaniciId == kullanici.BireyselKullaniciId); // Mevcut kullanıcıyı bulur
                if (currentUser != null)
                {
                    kullanici.Sifre = currentUser.Sifre; // Eski parolayı korur
                    kullanici.SifreKontrol = currentUser.SifreKontrol; // Eski parola kontrolünü korur
                    kullanici.KimlikNo = currentUser.KimlikNo; // Eski kimlik numarasını korur
                }
            }

            if (ModelState.IsValid)
            {
                var currentUser = _kullaniciRepository.BireyselKullanicilar.FirstOrDefault(u => u.BireyselKullaniciId == kullanici.BireyselKullaniciId); // Mevcut kullanıcıyı bulur
                if (currentUser == null)
                {
                    return NotFound(); // Kullanıcı bulunamazsa 404 hatası döner
                }
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
                    KimlikNo = currentUser.KimlikNo  // Eski kimlik numarasını kullanır
                };

                if (changePassword)
                {
                    userUpdate.Sifre = _passwordHasher.HashPassword(userUpdate, kullanici.Sifre); // Parolayı günceller
                    userUpdate.SifreKontrol = _passwordHasher.HashPassword(userUpdate, kullanici.SifreKontrol); // Parola kontrolünü günceller
                }

                _kullaniciRepository.KullaniciDuzenle(userUpdate); // Kullanıcıyı günceller

                var isUser = _kullaniciRepository.BireyselKullanicilar.FirstOrDefault(x => x.Eposta == kullanici.Eposta && x.Sifre == userUpdate.Sifre); // Güncellenmiş kullanıcıyı bulur
                var userClaims = new List<Claim>
                {
                    new (ClaimTypes.NameIdentifier, isUser!.BireyselKullaniciId.ToString()), // Kullanıcı ID'si
                    new (ClaimTypes.GivenName, isUser.Ad ?? ""), // Kullanıcı adı
                    new (ClaimTypes.Surname, isUser.Soyad ?? "") // Kullanıcı soyadı
                };
                var userIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // Kalıcı oturum açma
                };
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Mevcut oturumu sonlandırır
                await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(userIdentity),
                            authProperties
                            ); // Yeni oturumu başlatır
                return RedirectToAction("Index", "Home"); // Düzenleme sonrası ana sayfaya yönlendirir

            }
            return View(kullanici); // Model geçersizse düzenleme formunu yeniden gösterir
        }

        // Kullanıcı silme sayfasını görüntüler
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Geçersiz ID için 404 hatası döner
            }
            var user = await _kullaniciRepository.BireyselKullanicilar.FirstOrDefaultAsync(i => i.BireyselKullaniciId == id); // Kullanıcıyı ID ile bulur
            if (user == null)
            {
                return NotFound(); // Kullanıcı bulunamazsa 404 hatası döner
            }
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
            }); // Kullanıcı bilgilerini view model ile döner
        }

        // Kullanıcı silme işlemi
        [Authorize, HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var entityDelete = await _kullaniciRepository.BireyselKullanicilar.FirstOrDefaultAsync(k => k.BireyselKullaniciId == id); // Silinecek kullanıcıyı bulur

            if (entityDelete == null)
            {
                return NotFound(); // Kullanıcı bulunamazsa 404 hatası döner
            }
            _kullaniciRepository.KullaniciSil(entityDelete); // Kullanıcıyı veritabanından siler
            await HttpContext.SignOutAsync(); // Oturumu sonlandırır
            return RedirectToAction("Index", "Home"); // Silme sonrası ana sayfaya yönlendirir
        }

    }
}