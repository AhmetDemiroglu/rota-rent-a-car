using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Rent_A_Car.Entity;

namespace Rent_A_Car.Models
{
    public class AracViewModel
    {
        public int AracId { get; set; }
        public string? Marka { get; set; }
        public string? AracModel { get; set; }
        public string? Vites { get; set; }
        public string? Motor { get; set; }
        public string? Koltuk { get; set; }
        public string? Renk { get; set; }
        public string? Plaka { get; set; }
        public int Stok { get; set; }
        public int Km { get; set; }
        public bool? Musaitlik { get; set; }
        public decimal AracFiyat { get; set; }
        public List<Rezervasyon> Rezervasyonlar { get; set; } = new();
        public List<Arac> Araclar { get; set; } = new();
        public List<Sehir> Sehirler { get; set; } = new();
        public List<Kategori> Kategoriler { get; set; } = new();
        public int SehirId { get; set; }
        public Sehir? Sehir { get; set; }
        public int KategoriId { get; set; }
        public Kategori? Kategori { get; set; }
        public string? Resim { get; set; }
        public IFormFile? Image { get; set; }
        public string? SelectedKategori { get; set; }
    }
}
