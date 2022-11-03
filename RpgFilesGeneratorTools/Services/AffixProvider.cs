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
            await LoadAffixesAsync(cancellationToken).ConfigureAwait(false);
        }

        return _affixes.AsReadOnly();
    }

    private async Task LoadAffixesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var itemsFilePath = Path.Combine(_appConfig.AssetsFilesFolder, "Affixes.csv");

            var lines = await File.ReadAllLinesAsync(itemsFilePath, cancellationToken).ConfigureAwait(false);

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

                // TODO: improve the string manipulation
                var itemTypeString = infos[19].Trim().Replace(" ", "").Replace("eapons", "eapon").Replace("hields", "hield");
                var itemTypeString2 = infos[20].Trim().Replace(" ", "");
                var itemTypeString3 = infos[21].Trim().Replace(" ", "");
                var itemTypeString4 = infos[22].Trim().Replace(" ", "");
                var itemTypeString5 = infos[23].Trim().Replace(" ", "");
                var itemTypeString6 = infos[24].Trim().Replace(" ", "");

                if (string.IsNullOrWhiteSpace(infos[1]) ||
                    (!Enum.TryParse<ItemType>(itemTypeString, true, out _) && !Enum.TryParse<ItemSubtype>(itemTypeString, true, out _)) ||
                    !int.TryParse(infos[1], out var tier))
                {
                    continue; // Invalid affix: skip
                }

                List<ItemType> itemTypes = new();
                List<ItemSubtype> itemSubtypes = new();

                foreach (var value in new[] { itemTypeString, itemTypeString2, itemTypeString3, itemTypeString4, itemTypeString5, itemTypeString6 }
                             .Where(t => !string.IsNullOrWhiteSpace(t)))
                {
                    if (Enum.TryParse<ItemType>(value, true, out var itemType))
                    {
                        itemTypes.Add(itemType);
                    }
                    else if (Enum.TryParse<ItemSubtype>(value, true, out var itemSubtype))
                    {
                        itemSubtypes.Add(itemSubtype);
                    }
                }

                var lastAffix = _affixes[index];
                lastAffix.Rules.Add(CreateAffixRule(itemTypes, itemSubtypes, tier, infos));
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Load affixes failed");
            throw;
        }
    }

    private static AffixRule CreateAffixRule(IReadOnlyList<ItemType> itemTypes, IReadOnlyList<ItemSubtype> itemSubtypes, int tier, IReadOnlyList<string> infos)
    {
        int.TryParse(infos[2], out var isRare);
        int.TryParse(infos[3], out var isElite);
        int.TryParse(infos[5], out var itemLevelRequired);
        int.TryParse(infos[6], out var powerLevelRequired);
        int.TryParse(infos[7], out var maxLevel);
        int.TryParse(infos[8], out var frequency);
        int.TryParse(infos[9], out var group);

        return new AffixRule(
            itemTypes,
            itemSubtypes,
            tier,
            isRare == 1,
            isElite == 1,
            itemLevelRequired,
            powerLevelRequired,
            group,
            maxLevel,
            frequency,
            Modifier1: infos[10],
            Modifier1Min: infos[11],
            Modifier1Max: infos[12]);
    }
}
