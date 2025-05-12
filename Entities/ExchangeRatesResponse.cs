namespace api.Entities
{
          public record Valute(string ID, string Name, decimal Value);

        public record RatesResponse(DateTime Timestamp, ValueList Valute);

        public record ValueList(Valute USD, Valute EUR);
 
}
