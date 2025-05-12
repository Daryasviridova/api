namespace api.Entities
{
    public class Currency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CurrencyRate> CurrencyRates { get; set; }
    }
}
