using FinancialERP.Business.Services.Abstract;

namespace FinancialERP.API.BackgroundServices
{
    /// <summary>
    /// Günlük veri senkronizasyon servisi.
    /// Her gün saat 09:00'da (Türkiye saati) çalışır.
    /// - TCMB'den döviz kurlarını çeker
    /// - Barsoft'tan günlük satışları senkronize eder
    /// - Akıllı bildirimler üretir
    /// </summary>
    public class DailyDataSyncService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DailyDataSyncService> _logger;

        public DailyDataSyncService(IServiceProvider serviceProvider, ILogger<DailyDataSyncService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Günlük veri senkronizasyon servisi başlatıldı.");

            while (!stoppingToken.IsCancellationRequested)
            {
                // Türkiye saati ile 09:00'a kadar bekle
                var turkeyTime = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"));

                var nextRun = turkeyTime.Date.AddHours(9);
                if (turkeyTime.Hour >= 9)
                    nextRun = nextRun.AddDays(1);

                var delay = nextRun - turkeyTime;
                _logger.LogInformation($"Sonraki senkronizasyon: {nextRun:dd.MM.yyyy HH:mm} ({delay.TotalHours:F1} saat sonra)");

                await Task.Delay(delay, stoppingToken);

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var macroService = scope.ServiceProvider.GetRequiredService<IMacroEconomyIntegrationService>();

                    _logger.LogInformation("Günlük senkronizasyon çalıştırılıyor...");
                    await macroService.SyncDailyRatesAsync();
                    await macroService.SyncInflationDataAsync();
                    _logger.LogInformation("Günlük senkronizasyon tamamlandı.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Günlük senkronizasyonda hata.");
                }
            }
        }
    }
}
