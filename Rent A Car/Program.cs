using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rent_A_Car.Data.Abstract;
using Rent_A_Car.Data.Concrete.EfCore;
using Rent_A_Car.Entity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Context>(options =>
{
    //var config = builder.configuration;
    //var connectionstring = config.getconnectionstring("database");
    //options.usesqlite(connectionstring);

    options.UseSqlite(builder.Configuration["ConnectionStrings:database"]);   // Yukarýdaki iþlemin farklý kullanýmý.

});

builder.Services.AddScoped<IRezervasyonRepository, EfRezervasyonRepository>();
builder.Services.AddScoped<IAracRepository, EfAracRepository>();
builder.Services.AddScoped<IBireyselKullaniciRepository, EfBireyselKullaniciRepository>();
builder.Services.AddScoped<IKurumsalKullaniciRepository, EfKurumsalKullaniciRepository>();
builder.Services.AddScoped<IIletisimRepository, EfIletisimRepository>();
builder.Services.AddScoped<ISehirRepository, EfSehirRepository>();
builder.Services.AddScoped<IKategoriRepository, EfKategoriRepository>();
builder.Services.AddScoped<IPasswordHasher<BireyselKullanici>, PasswordHasher<BireyselKullanici>>();
builder.Services.AddScoped<IPasswordHasher<KurumsalKullanici>, PasswordHasher<KurumsalKullanici>>();



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/BireyselKullanici/Login";
    options.LogoutPath = "/BireyselKullanici/Logout";
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
