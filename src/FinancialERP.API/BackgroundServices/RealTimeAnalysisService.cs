using FinancialERP.Business.Services.Abstract;

namespace FinancialERP.API.BackgroundServices
{
    /// <summary>
    /// Periyodik analiz servisi - her 4 saatte bir çalışır.
    /// Kâr marjlarını, nakit akışını ve kritik uyarıları günceller.
    /// </summary>
    public class RealTimeAnalysisService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RealTimeAnalysisService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(4);

        public RealTimeAnalysisService(IServiceProvider serviceProvider, ILogger<RealTimeAnalysisService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Periyodik analiz servisi başlatıldı (4 saatlik döngü).");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var analysisService = scope.ServiceProvider.GetRequiredService<IFinancialAnalysisService>();

                    _logger.LogInformation("Periyodik analiz çalıştırılıyor...");
                    await analysisService.GenerateSmartNotificationsAsync();
                    _logger.LogInformation("Periyodik analiz tamamlandı.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Periyodik analizde hata.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
