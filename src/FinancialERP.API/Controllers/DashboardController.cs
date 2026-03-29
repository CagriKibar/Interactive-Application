using FinancialERP.Business.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace FinancialERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IFinancialAnalysisService _analysisService;

        public DashboardController(IFinancialAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }

        /// <summary>Yönetici özet panosu - tüm analizleri birleştirir</summary>
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var dashboard = await _analysisService.GenerateExecutiveDashboardAsync();
            return Ok(dashboard);
        }
    }
}
