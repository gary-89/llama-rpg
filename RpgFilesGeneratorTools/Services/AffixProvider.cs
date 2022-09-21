using Microsoft.Extensions.Logging;
using RpgFilesGeneratorTools.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace RpgFilesGeneratorTools.Services;

internal sealed class AffixProvider : IAffixProvider
{
    private const char CsvSeparator = ',';

    private readonly AppConfig _appConfig;
    private readonly ILogger<AffixProvider> _logger;
    private readonly List<Affix> _affixes = new();

    public AffixProvider(AppConfig appConfig, ILogger<AffixProvider> logger)
    {
        _appConfig = appConfig;
        _logger = logger;
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
            var itemsFilePath = Path.Combine(_appConfig.AssetsFilesFolder, "Affixes.csv");

            var lines = await File.ReadAllLinesAsync(itemsFilePath, cancellationToken);

            var index = -1;

            foreach (var line in lines[1..])
            {
                var infos = line.Split(CsvSeparator);

                if (!string.IsNullOrWhiteSpace(infos[0]))
                {
                    var affix = new Affix(infos[0]);
                    _affixes.Add(affix);
                    index++;
                    continue;
                }

                var itemType = infos[19];

                if (string.IsNullOrWhiteSpace(infos[1]) || string.IsNullOrWhiteSpace(itemType) || !int.TryParse(infos[1], out var tier))
                {
                    continue;
                }

                var lastAffix = _affixes[index];
                lastAffix.ItemTypes.Add(CreateAffixItemType(itemType, tier, infos));
            }

            //await foreach (var item in FetchAsync(cancellationToken))
            //{
            //    _affixes.Add(item);
            //}
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Load affixes failed");
            throw;
        }
    }

    private AffixItemType CreateAffixItemType(string itemType, int tier, string[] infos)
    {
        int.TryParse(infos[2], out var isRare);
        int.TryParse(infos[3], out var isElite);
        int.TryParse(infos[5], out var itemLevel);
        int.TryParse(infos[7], out var frequency);

        return new AffixItemType(
            itemType,
            tier,
            isRare == 1,
            isElite == 1,
            itemLevel,
            frequency,
            modifier1: infos[13],
            modifier1Min: infos[11],
            modifier1Max: infos[12]);
    }

    private static async IAsyncEnumerable<Affix> FetchAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.Delay(2000, cancellationToken).ConfigureAwait(false);
        yield return new Affix("Fire resistance");
        await Task.Delay(2000, cancellationToken).ConfigureAwait(false);
        yield return new Affix("Cold resistance");
        await Task.Delay(2000, cancellationToken).ConfigureAwait(false);
        yield return new Affix("Lighting resistance");
    }
}
