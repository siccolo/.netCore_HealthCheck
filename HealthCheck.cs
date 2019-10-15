using System;
using System.Collections.Generic;
using System.Linq;
//
using System.Threading;
using System.Threading.Tasks;
//
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
//
namespace WebAPI_v1.Helpers
{
    public class HealthCheck: IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default(CancellationToken))
        {
            var healthCheckResultHealthy = true;

            if (healthCheckResultHealthy)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("OK"));
            }

            return Task.FromResult(
                HealthCheckResult.Unhealthy("Failed"));
        }
    }
}
