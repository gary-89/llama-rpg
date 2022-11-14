using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LlamaRpg.App.Services;

internal sealed class UpdateDetector : IUpdatesDetector
{
    private const char Separator = '.';

    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<UpdateDetector> _logger;

    public UpdateDetector(IHttpClientFactory clientFactory, ILogger<UpdateDetector> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public async Task<Version?> GetLastVersionAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var client = _clientFactory.CreateClient(AppClientFactoryNames.AppUpdatesClientName);

            var result = await client.GetStringAsync("app/llamarpg/lastversion.txt", cancellationToken).ConfigureAwait(false);

            var numbers = result.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

            if (!int.TryParse(numbers[0], out var major)
                || !int.TryParse(numbers[1], out var minor)
                || !int.TryParse(numbers[2], out var build))
            {
                return null;
            }

            return new Version(major, minor, build, 0);
        }
        catch (Exception exception)
        {
            _logger.LogError("Cannot get last app version", exception);
            return null;
        }
    }
}
