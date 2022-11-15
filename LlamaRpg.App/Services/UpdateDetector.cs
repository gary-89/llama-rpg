using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LlamaRpg.App.Services;

internal sealed class UpdateDetector : IUpdatesDetector
{
    private const char Separator = '.';
    private const string DownloadLastVersion = "app/llamarpg/lastversion.txt";

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

            var result = await client.GetStringAsync(DownloadLastVersion, cancellationToken).ConfigureAwait(false);

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

    public async Task<string?> DownloadLastVersionAsync(CancellationToken cancellationToken)
    {
        try
        {
            var lastVersion = await GetLastVersionAsync(cancellationToken).ConfigureAwait(false);

            if (lastVersion is null)
            {
                return default;
            }

            using var client = _clientFactory.CreateClient(AppClientFactoryNames.AppUpdatesClientName);

            var filename = $"LlamaRpg_app_{lastVersion.Major}.{lastVersion.Minor}.{lastVersion.Build}.7z";

            var url = $"app/llamarpg/{filename}";

            var response = await client
                .GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            if (response.IsSuccessStatusCode is false)
            {
                _logger.LogError("Cannot find the last available version: {Url}", url);
                return default;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

            var destinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), filename);

            await using var fileStream = File.Open(destinationPath, FileMode.Create, FileAccess.Write);

            await stream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);

            return destinationPath;
        }
        catch (Exception exception)
        {
            _logger.LogError("Cannot get last version: {Message}", exception.Message);
            return default;
        }
    }
}
