using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace POC_Grafana.BackgroundServices
{
    public class TestarLibLogsBackgroundService : BackgroundService
    {
        private readonly ILogger<TestarLibLogsBackgroundService> _logger;

        public TestarLibLogsBackgroundService(ILogger<TestarLibLogsBackgroundService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("GRAFANA | Testando...");
                await Task.Delay(1000);
            }
            
        }
    }
}
