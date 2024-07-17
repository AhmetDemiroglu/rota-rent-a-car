namespace Rent_A_Car.Models
{
    public class IletisimViewModel
    {
        public int IletisimId { get; set; }
        public string? IletisimAdSoyad { get; set; }
        public string? IletisimEposta { get; set; }
        public string? Telefon { get; set; }
        public string? Mesaj { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentDate { get; set; }
    }
}
