using api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.Extensions.Hosting;

namespace api.infrastructure
{
    public class Job : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public Job(IServiceProvider serviceProvider, ILogger logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CurrencyDbContext>();
                var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();

                // Удалить предыдущие данные
                dbContext.CurrencyRates.RemoveRange(dbContext.CurrencyRates);
                dbContext.Currencies.RemoveRange(dbContext.Currencies);
                await dbContext.SaveChangesAsync();

                var usdCurrency = new Currency { Name = "USD" };
                var eurCurrency = new Currency { Name = "EUR" };
                await dbContext.Currencies.AddRangeAsync(usdCurrency, eurCurrency);
                await dbContext.SaveChangesAsync();

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        _logger.LogInformation("Запрос данных о курсах валют.");
                    var json = await httpClient.GetFromJsonAsync<RatesResponse>("https://www.cbr-xml-daily.ru/daily_json.js");

                    dbContext.CurrencyRates.Add(new CurrencyRate
                    {
                        CurrencyId = usdCurrency.Id,
                        Rate = json.Valute.USD.Value,
                        Date = DateTime.UtcNow
                    });
                    dbContext.CurrencyRates.Add(new CurrencyRate
                    {
                        CurrencyId = eurCurrency.Id,
                        Rate = json.Valute.EUR.Value,
                        Date = DateTime.UtcNow
                    });


                    await dbContext.SaveChangesAsync();
                        _logger.LogInformation("Данные о курсах валют успешно сохранены.");
                    }
                    catch (HttpRequestException httpEx)
                    {
                        // Логирование ошибки запроса
                        _logger.LogError(httpEx, "Ошибка запроса к API: {Message}", httpEx.Message);
                    }
                    catch (Exception ex)
                    {
                        // Логирование других ошибок
                        _logger.LogError(ex, "Произошла ошибка: {Message}", ex.Message);
                    }

                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}

