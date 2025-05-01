using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Web.Options;

internal class RedditOptionsHealthCheck : IHealthCheck
{
    #region Private variables

    private readonly IOptions<RedditOptions> _redditOptions;

    #endregion

    #region Constructor

    public RedditOptionsHealthCheck(IOptions<RedditOptions> redditOptions)
    {
        if (redditOptions == null)
        {
            throw new ArgumentNullException(nameof(redditOptions));
        }

        _redditOptions = redditOptions;
    }

    #endregion

    #region Methods

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        RedditOptions redditOptions = _redditOptions?.Value;
        if (redditOptions == null)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"{nameof(RedditOptions)} are misconfigured."));
        }

        if (string.IsNullOrWhiteSpace(redditOptions.ClientId))
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"{nameof(redditOptions.ClientId)} in {nameof(RedditOptions)} are misconfigured."));
        }

        if (string.IsNullOrWhiteSpace(redditOptions.ClientSecret))
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"{nameof(redditOptions.ClientSecret)} in {nameof(RedditOptions)} are misconfigured."));
        }

        return Task.FromResult(HealthCheckResult.Healthy());
    }

    #endregion
}