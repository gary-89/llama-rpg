using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RpgFilesGeneratorTools.Models;

namespace RpgFilesGeneratorTools.Services;

internal sealed class ItemProvider : IItemProvider
{
    private const char CsvSeparator = ',';

    private readonly AppConfig _appConfig;
    private readonly ILogger<ItemProvider> _logger;
    private readonly List<Item> _items = new();

    private List<ItemType>? _itemTypes;

    public ItemProvider(AppConfig appConfig, ILogger<ItemProvider> logger)
    {
        _appConfig = appConfig;
        _logger = logger;
    }

    public async ValueTask<IReadOnlyList<Item>> GetItemsAsync(CancellationToken cancellationToken)
    {
        if (_items.Count == 0)
        {
            await LoadItemsAsync(cancellationToken).ConfigureAwait(false);
        }

        return _items;
    }

    public async ValueTask<IReadOnlyList<ItemType>> GetItemTypesAsync(CancellationToken cancellationToken)
    {
        if (_itemTypes is not null)
        {
            return _itemTypes.AsReadOnly();
        }

        var items = await GetItemsAsync(cancellationToken).ConfigureAwait(false);
        _itemTypes = items.Select(x => x.Type).Distinct().ToList();

        return _itemTypes;
    }

    private async Task LoadItemsAsync(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Loading items...");

            var itemsFilePath = Path.Combine(_appConfig.AssetsFilesFolder, "Items.csv");
            var lines = await File.ReadAllLinesAsync(itemsFilePath, cancellationToken).ConfigureAwait(false);

            foreach (var line in lines[1..])
            {
                AddItem(line);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to load items");
            throw;
        }
        finally
        {
            _logger.LogInformation("{NumberOfItems} items loaded in {ElapsedTimeInMillisecond} ms", _items.Count, stopwatch.ElapsedMilliseconds);
        }
    }

    private void AddItem(string line)
    {
        var infos = line.Split(CsvSeparator);

        int.TryParse(infos[4].Replace("%", ""), out var statusChancePercentage);
        int.TryParse(infos[6].Replace("%", ""), out var status2ChancePercentage);
        int.TryParse(infos[7].Replace("plvl *", ""), out var requiredStrength);
        int.TryParse(infos[8].Replace("plvl *", ""), out var requiredDexterity);
        int.TryParse(infos[9].Replace("plvl *", ""), out var requiredIntelligence);
        int.TryParse(infos[10], out var sockets);
        int.TryParse(infos[11], out var speed);
        int.TryParse(infos[12], out var minDamage);
        int.TryParse(infos[13], out var maxDamage);
        int.TryParse(infos[14], out var minDefense);
        int.TryParse(infos[15], out var maxDefense);
        int.TryParse(infos[16], out var minBlock);
        int.TryParse(infos[17], out var maxBlock);

        if (!Enum.TryParse<ItemType>(infos[1].Trim(), true, out var type) ||
            !Enum.TryParse<ItemSubtype>(infos[2].Trim(), true, out var subtype))
        {
            return;
        }

        var item = new Item(
            name: infos[0].Trim(),
            type: type,
            subtype: subtype,
            status: infos[3].Trim(),
            statusChance: statusChancePercentage,
            status2: infos[5].Trim(),
            status2Chance: status2ChancePercentage,
            requiredStrength: requiredStrength,
            requiredDexterity: requiredDexterity,
            requiredIntelligence: requiredIntelligence,
            minDamage: minDamage,
            maxDamage: maxDamage,
            minDefense: minDefense,
            maxDefense: maxDefense,
            minBlock: minBlock,
            maxBlock: maxBlock,
            totalMinBlock: infos[18],
            totalMaxBlock: infos[19],
            totalMin: infos[20],
            totalMax: infos[21],
            sockets: sockets,
            speed: speed);

        _items.Add(item);
    }
}
