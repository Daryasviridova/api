using api.Entities;
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
        public async Task<ActionResult<IEnumerable<Currency>>> GetAllCurrencies()
        {
            var currencies = await _context.Currencies.ToListAsync();
            return Ok(currencies);
        }

        // Метод для получения последней записи о валюте
        [HttpGet("latest")]
        public async Task<ActionResult<Currency>> GetLatestCurrency()
        {
            var latestCurrency = await _context.Currencies
                .OrderByDescending(c => c.Id)  // Используется Id для получения последней записи
                .FirstOrDefaultAsync();

            if (latestCurrency == null)
            {
                return NotFound("Нет записей о валюте.");
            }

            return Ok(latestCurrency);
        }

    }
}
