namespace api.Entities
{
    public class CurrencyRate
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public decimal? Rate { get; set; }
        public DateTime? Date { get; set; }

        public Currency Currency { get; set; }
    }
}
