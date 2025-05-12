using api.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyDbContext _context;

        public CurrencyController(CurrencyDbContext context)
        {
            _context = context;
        }

        // Метод для получения всех валют
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CurrencyWithRateDTO>>> GetAllCurrencies(int limit = 50)
        {
            var currenciesWithRates = await _context.Currencies
                .Take(limit)
                .GroupJoin(//объединение таблиц для корректтного представления
                    _context.CurrencyRates,
                    currency => currency.Id,
                    rate => rate.CurrencyId,
                    (currency, rates) => new CurrencyWithRateDTO
                    {
                        Id = currency.Id,
                        Name = currency.Name,
                        LatestRate = rates.OrderByDescending(r => r.Date).Select(r => r.Rate).FirstOrDefault(),
                        RateDate = rates.OrderByDescending(r => r.Date).Select(r => r.Date).FirstOrDefault()
                    })
                .ToListAsync();

            return Ok(currenciesWithRates);
        }


        // Метод для получения последней записи о валюте
        [HttpGet("latest")]
        public async Task<ActionResult<CurrencyWithRateDTO>> GetLatestCurrency()
        {
            var latestCurrency = await _context.Currencies
                .OrderByDescending(c => c.Id)
                .Select(c => new CurrencyWithRateDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    // Получаем последний курс
                    LatestRate = _context.CurrencyRates
                        .Where(cr => cr.CurrencyId == c.Id)
                        .OrderByDescending(cr => cr.Date)
                        .Select(cr => cr.Rate)
                        .FirstOrDefault(),
                    RateDate = _context.CurrencyRates
                        .Where(cr => cr.CurrencyId == c.Id)
                        .OrderByDescending(cr => cr.Date)
                        .Select(cr => cr.Date)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (latestCurrency == null)
            {
                return NotFound("Нет записей о валюте.");
            }

            return Ok(latestCurrency);
        }



    }
}
