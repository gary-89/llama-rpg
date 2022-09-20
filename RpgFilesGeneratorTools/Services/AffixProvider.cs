using Microsoft.Extensions.Logging;
using RpgFilesGeneratorTools.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace RpgFilesGeneratorTools.Services;

internal class AffixProvider : IAffixProvider
{
    private readonly ILogger<AffixProvider> _logger;
    private readonly List<Affix> _affixes = new();

    public AffixProvider(ILogger<AffixProvider> logger)
    {
        _logger = logger;
        _logger.LogInformation("Instantiating affix provider");
    }

    public async ValueTask<IReadOnlyList<Affix>> GetAffixesAsync(CancellationToken cancellationToken)
    {
        if (_affixes.Count == 0)
        {
            await LoadAffixesAsync(cancellationToken);
        }

        return _affixes.AsReadOnly();
    }

    private async Task LoadAffixesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var item in FetchAsync(cancellationToken))
            {
                _affixes.Add(item);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Load affixes failed");
            throw;
        }
    }

    private static async IAsyncEnumerable<Affix> FetchAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.Delay(2000, cancellationToken).ConfigureAwait(false);
        yield return new Affix("Fire resistance", 0, 15, 20, "Gloves");
        await Task.Delay(2000, cancellationToken).ConfigureAwait(false);
        yield return new Affix("Cold resistance", 10, 15, 20, "Boots");
        await Task.Delay(2000, cancellationToken).ConfigureAwait(false);
        yield return new Affix("Lighting resistance", 0, 20, 20, "Boots");
    }
}
