namespace api.DTO
{
    public class CurrencyWithRateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? LatestRate { get; set; }  // Последний курс
        public DateTime? RateDate { get; set; } // Дата последнего курса
    }
}
