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
    public class KurumsalKullaniciController : Controller
    {
        private readonly IKurumsalKullaniciRepository _kurumsalkullaniciRepository; // Kurumsal kullanıcı işlemleri için repository
        private readonly IPasswordHasher<KurumsalKullanici> _passwordHasher; // Parola hashleme için

        public KurumsalKullaniciController(IKurumsalKullaniciRepository kurumsalkullaniciRepository, IRezervasyonRepository rezervasyonRepository, IAracRepository aracRepository, IPasswordHasher<KurumsalKullanici> passwordHasher)
        {
            _kurumsalkullaniciRepository = kurumsalkullaniciRepository;
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
        public IActionResult Register(KurumsalKullaniciViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_kurumsalkullaniciRepository.KullaniciDurumu(model.FirmaEposta!))
                {
                    ModelState.AddModelError("", "Bu e-posta zaten kullanılıyor."); // E-posta adresi zaten kayıtlıysa hata mesajı ekler
                    return View(model);
                }

                // Yeni kurumsal kullanıcı oluşturur
                var entity = new KurumsalKullanici
                {
                    KurumsalKullaniciId = model.KurumsalKullaniciId,
                    FirmaEposta = model.FirmaEposta,
                    FirmaAdi = model.FirmaAdi,
                    FirmaTelefon = model.FirmaTelefon,
                    FaturaAdresi = model.FaturaAdresi,
                    FirmaIl = model.FirmaIl,
                    FirmaIlce = model.FirmaIlce,
                    VergiNo = model.VergiNo,
                    VergiDairesi = model.VergiDairesi,
                    VergiIl = model.VergiIl,
                    VergiIlce = model.VergiIlce,
                };

                entity.FirmaSifre = _passwordHasher.HashPassword(entity, model.FirmaSifre); // Parolayı hashler
                entity.FirmaSifreKontrol = entity.FirmaSifre; // Parola kontrolü için hashlenmiş parolayı ekler

                _kurumsalkullaniciRepository.KullaniciKayit(entity); // Yeni kullanıcıyı veritabanına ekler
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
        public async Task<IActionResult> Login(KurumsalLoginViewModel kullanici)
        {
            if (ModelState.IsValid)
            {
                var isUser = _kurumsalkullaniciRepository.KurumsalKullanicilar.FirstOrDefault(x => x.FirmaEposta == kullanici.FirmaEposta); // Kullanıcıyı e-posta ile bulur
                if (isUser != null && _passwordHasher.VerifyHashedPassword(isUser, isUser.FirmaSifre, kullanici.FirmaSifre) == PasswordVerificationResult.Success)
                {
                    // Kullanıcı giriş bilgilerini ayarlar
                    var userClaims = new List<Claim>
                    {
                        new (ClaimTypes.NameIdentifier, isUser.KurumsalKullaniciId.ToString()), // Kullanıcı ID'si
                        new (ClaimTypes.GivenName, isUser.FirmaAdi ?? ""), // Firma adı
                        new ("KullaniciTipi", "Kurumsal") // Kullanıcı tipi
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

        // Kurumsal kullanıcı düzenleme sayfasını görüntüler
        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Geçersiz ID için 404 hatası döner
            }
            var kullanici = _kurumsalkullaniciRepository.KurumsalKullanicilar.FirstOrDefault(i => i.KurumsalKullaniciId == id); // Kullanıcıyı ID ile bulur
            if (kullanici == null)
            {
                return NotFound(); // Kullanıcı bulunamazsa 404 hatası döner
            }
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
            }); // Kullanıcı bilgilerini view model ile döner
        }

        // Kurumsal kullanıcı düzenleme işlemi
        [Authorize, HttpPost]
        public async Task<IActionResult> Edit(KurumsalKullaniciViewModel kullanici, bool changePassword)
        {
            if (!changePassword)
            {
                ModelState.Remove("FirmaSifre");
                ModelState.Remove("FirmaSifreKontrol");
                var currentUser = _kurumsalkullaniciRepository.KurumsalKullanicilar.FirstOrDefault(u => u.KurumsalKullaniciId == kullanici.KurumsalKullaniciId); // Mevcut kullanıcıyı bulur
                if (currentUser != null)
                {
                    kullanici.FirmaSifre = currentUser.FirmaSifre; // Eski parolayı korur
                    kullanici.FirmaSifreKontrol = currentUser.FirmaSifreKontrol; // Eski parola kontrolünü korur
                }
            }

            if (ModelState.IsValid)
            {
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
                    userUpdate.FirmaSifre = _passwordHasher.HashPassword(userUpdate, kullanici.FirmaSifre); // Parolayı günceller
                    userUpdate.FirmaSifreKontrol = _passwordHasher.HashPassword(userUpdate, kullanici.FirmaSifreKontrol); // Parola kontrolünü günceller
                }

                _kurumsalkullaniciRepository.KullaniciDuzenle(userUpdate); // Kullanıcıyı günceller

                var isUser = _kurumsalkullaniciRepository.KurumsalKullanicilar.FirstOrDefault(x => x.FirmaEposta == kullanici.FirmaEposta && x.FirmaSifre == userUpdate.FirmaSifre); // Güncellenmiş kullanıcıyı bulur
                var userClaims = new List<Claim>
                {
                    new (ClaimTypes.NameIdentifier, isUser!.KurumsalKullaniciId.ToString()), // Kullanıcı ID'si
                    new (ClaimTypes.GivenName, isUser.FirmaAdi ?? ""), // Firma adı
                    new ("KullaniciTipi", "Kurumsal") // Kullanıcı tipi
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

        // Kurumsal kullanıcı silme sayfasını görüntüler
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Geçersiz ID için 404 hatası döner
            }
            var user = await _kurumsalkullaniciRepository.KurumsalKullanicilar.FirstOrDefaultAsync(i => i.KurumsalKullaniciId == id); // Kullanıcıyı ID ile bulur
            if (user == null)
            {
                return NotFound(); // Kullanıcı bulunamazsa 404 hatası döner
            }
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
            }); // Kullanıcı bilgilerini view model ile döner
        }

        // Kurumsal kullanıcı silme işlemi
        [Authorize, HttpPost]
        public async Task<IActionResult> Delete(KurumsalKullaniciViewModel kullanici)
        {
            var entityDelete = await _kurumsalkullaniciRepository.KurumsalKullanicilar.FirstOrDefaultAsync(k => k.KurumsalKullaniciId == kullanici.KurumsalKullaniciId); // Silinecek kullanıcıyı bulur

            if (entityDelete == null)
            {
                return NotFound(); // Kullanıcı bulunamazsa 404 hatası döner
            }
            _kurumsalkullaniciRepository.KullaniciSil(entityDelete); // Kullanıcıyı veritabanından siler
            return RedirectToAction("Index", "Home"); // Silme sonrası ana sayfaya yönlendirir
        }

    }
}