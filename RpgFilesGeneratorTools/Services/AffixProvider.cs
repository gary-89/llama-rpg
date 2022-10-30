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

                // TODO: improve the string manipulation
                var itemTypeString = infos[19].Trim().Replace(" ", "").Replace("eapons", "eapon").Replace("hields", "hield");
                var itemTypeString2 = infos[20].Trim().Replace(" ", "").Replace("eapons", "eapon").Replace("hields", "hield");
                var itemTypeString3 = infos[21].Trim().Replace(" ", "").Replace("eapons", "eapon").Replace("hields", "hield");
                var itemTypeString4 = infos[22].Trim().Replace(" ", "").Replace("eapons", "eapon").Replace("hields", "hield");
                var itemTypeString5 = infos[23].Trim().Replace(" ", "").Replace("eapons", "eapon").Replace("hields", "hield");
                var itemTypeString6 = infos[24].Trim().Replace(" ", "").Replace("eapons", "eapon").Replace("hields", "hield");
                var itemTypeString7 = infos[25].Trim().Replace(" ", "").Replace("eapons", "eapon").Replace("hields", "hield");
                var itemTypeString8 = infos[26].Trim().Replace(" ", "").Replace("eapons", "eapon").Replace("hields", "hield");

                if (string.IsNullOrWhiteSpace(infos[1]) || !Enum.TryParse<ItemType>(itemTypeString, true, out _) || !int.TryParse(infos[1], out var tier))
                {
                    continue;
                }

                var itemTypes =
                    new[] { itemTypeString, itemTypeString2, itemTypeString3, itemTypeString4, itemTypeString5, itemTypeString6, itemTypeString7, itemTypeString8 }
                        .Where(t => !string.IsNullOrWhiteSpace(t) && Enum.TryParse<ItemType>(itemTypeString, true, out _))
                        .Select(x =>
                        {
                            Enum.TryParse<ItemType>(x, true, out var itemType);
                            return itemType;
                        }).ToList();

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

    private static AffixRule CreateAffixRule(IReadOnlyList<ItemType> itemTypes, int tier, IReadOnlyList<string> infos)
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
