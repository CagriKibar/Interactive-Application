using FinancialERP.Business.Services.Abstract;
using FinancialERP.Entity.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FinancialERP.API.Controllers
{
    [ApiController]
    [Route("api/macro")]
    public class MacroEconomyController : ControllerBase
    {
        private readonly IMacroEconomyIntegrationService _macroService;

        public MacroEconomyController(IMacroEconomyIntegrationService macroService)
        {
            _macroService = macroService;
        }

        [HttpGet("exchange-rates/current")]
        public async Task<IActionResult> GetCurrentRates()
        {
            var usd = await _macroService.GetCurrentExchangeRateAsync(CurrencyType.USD);
            var eur = await _macroService.GetCurrentExchangeRateAsync(CurrencyType.EUR);
            return Ok(new { USD = usd, EUR = eur });
        }

        [HttpGet("exchange-rates/history")]
        public async Task<IActionResult> GetRateHistory(
            [FromQuery] CurrencyType currency = CurrencyType.USD,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var fromDate = from ?? DateTime.UtcNow.AddDays(-30);
            var toDate = to ?? DateTime.UtcNow;
            var rates = await _macroService.GetExchangeRateHistoryAsync(currency, fromDate, toDate);
            return Ok(rates);
        }

        [HttpGet("inflation/latest")]
        public async Task<IActionResult> GetLatestInflation()
        {
            var data = await _macroService.GetLatestInflationDataAsync();
            return Ok(data);
        }

        [HttpPost("sync")]
        public async Task<IActionResult> TriggerSync()
        {
            await _macroService.SyncDailyRatesAsync();
            return Ok(new { Message = "Senkronizasyon tamamlandı." });
        }
    }
}
