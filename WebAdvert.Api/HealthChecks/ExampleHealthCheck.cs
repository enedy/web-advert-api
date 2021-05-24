using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;
using WebAdvert.Api.Services;

namespace WebAdvert.Api.HealthChecks
{
    public class ExampleHealthCheck : IHealthCheck
    {
        private readonly IAdvertStorageService _storageService;

        public ExampleHealthCheck(IAdvertStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            //var healthCheckResultHealthy = true;

            //if (healthCheckResultHealthy)
            //{
            //    return Task.FromResult(
            //        HealthCheckResult.Healthy("A healthy result."));
            //}

            var isStorageOk = await _storageService.CheckHealthAsync();

            if (isStorageOk)
                return HealthCheckResult.Healthy("A healthy result.");
            else
                return HealthCheckResult.Unhealthy("A Unhealthy result.");

            //return Task.FromResult(
            //    new HealthCheckResult(context.Registration.FailureStatus,
            //    "An unhealthy result."));
        }
    }
}
