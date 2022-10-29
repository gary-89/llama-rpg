using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RpgFilesGeneratorTools.Models;

namespace RpgFilesGeneratorTools.Services;

internal sealed class AffixProvider : IAffixProvider
{
    private const char CsvSeparator = ',';

    private readonly AppConfig _appConfig;
    private readonly List<Affix> _affixes = new();
    private readonly ILogger<AffixProvider> _logger;

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
                var itemType2 = infos[20];
                var itemType3 = infos[21];
                var itemType4 = infos[22];
                var itemType5 = infos[23];
                var itemType6 = infos[24];
                var itemType7 = infos[25];
                var itemType8 = infos[26];

                if (string.IsNullOrWhiteSpace(infos[1]) || string.IsNullOrWhiteSpace(itemType) || !int.TryParse(infos[1], out var tier))
                {
                    continue;
                }

                var itemTypes =
                    new[] { itemType, itemType2, itemType3, itemType4, itemType5, itemType6, itemType7, itemType8 }
                        .Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

                var lastAffix = _affixes[index];
                lastAffix.Rules.Add(CreateAffixRule(itemTypes, tier, infos));
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Load affixes failed");
            throw;
        }
    }

    private static AffixRule CreateAffixRule(IReadOnlyList<string> itemTypes, int tier, IReadOnlyList<string> infos)
    {
        int.TryParse(infos[2], out var isRare);
        int.TryParse(infos[3], out var isElite);
        int.TryParse(infos[5], out var itemLevel);
        int.TryParse(infos[7], out var frequency);

        return new AffixRule(
            itemTypes,
            tier,
            isRare == 1,
            isElite == 1,
            itemLevel,
            frequency,
            Modifier1: infos[10],
            Modifier1Min: infos[11],
            Modifier1Max: infos[12]);
    }
}
