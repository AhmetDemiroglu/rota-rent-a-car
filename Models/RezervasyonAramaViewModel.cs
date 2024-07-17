using Microsoft.AspNetCore.Mvc.Rendering;
using Rent_A_Car.Entity;

namespace Rent_A_Car.Models
{
        public class RezervasyonAramaViewModel
        {
            public int TeslimYeriId { get; set; }
            public int IadeYeriId { get; set; }
            public DateTime TeslimTarih { get; set; }
            public DateTime IadeTarih { get; set; }
            public int KategoriId { get; set; }
            public IEnumerable<SelectListItem> TeslimYeriSecenekleri { get; set; } = new List<SelectListItem>();
            public IEnumerable<SelectListItem> IadeYeriSecenekleri { get; set; } = new List<SelectListItem>();
            public IEnumerable<SelectListItem> KategoriSecenekleri { get; set; } = new List<SelectListItem>();
            public IEnumerable<Arac> Araclar { get; set; } = new List<Arac>();

    }
}

