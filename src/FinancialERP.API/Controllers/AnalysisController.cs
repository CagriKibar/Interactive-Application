using FinancialERP.Business.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace FinancialERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly IFinancialAnalysisService _analysisService;

        public AnalysisController(IFinancialAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }

        [HttpGet("profit-margins")]
        public async Task<IActionResult> GetProfitMargins()
        {
            var result = await _analysisService.AnalyzeProfitMarginsAsync();
            return Ok(result);
        }

        [HttpGet("stock-optimization")]
        public async Task<IActionResult> GetStockOptimization()
        {
            var result = await _analysisService.AnalyzeStockOptimizationAsync();
            return Ok(result);
        }

        [HttpGet("cash-flow-health")]
        public async Task<IActionResult> GetCashFlowHealth()
        {
            var result = await _analysisService.AnalyzeCashFlowHealthAsync();
            return Ok(result);
        }

        [HttpGet("macro-summary")]
        public async Task<IActionResult> GetMacroSummary()
        {
            var result = await _analysisService.GetMacroEconomicSummaryAsync();
            return Ok(result);
        }
    }
}
