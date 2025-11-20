using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PROG6212_POE.Services
{
    public class ClaimAutomationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ClaimAutomationService> _logger;

        public ClaimAutomationService(IServiceProvider serviceProvider, ILogger<ClaimAutomationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Claim Automation Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var claimService = scope.ServiceProvider.GetRequiredService<IClaimService>();

                    // Auto-approve eligible claims
                    await claimService.AutoApproveClaimsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in automated claim processing");
                }

                // Wait for 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            _logger.LogInformation("Claim Automation Service stopped.");
        }
    }
}