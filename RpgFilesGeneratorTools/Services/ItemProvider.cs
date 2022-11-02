using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RpgFilesGeneratorTools.Models;
using RpgFilesGeneratorTools.Models.ItemTypes;

namespace RpgFilesGeneratorTools.Services;

internal sealed class ItemProvider : IItemProvider
{
    private const char CsvSeparator = ',';

    private readonly AppConfig _appConfig;
    private readonly ItemValidator _itemValidator;
    private readonly ILogger<ItemProvider> _logger;
    private readonly List<ItemBase> _items2 = new();

    private List<ItemType>? _itemTypes;

    public ItemProvider(AppConfig appConfig, ItemValidator itemValidator, ILogger<ItemProvider> logger)
    {
        _appConfig = appConfig;
        _itemValidator = itemValidator;
        _logger = logger;
    }


    public async Task<bool> AddItemAsync(ItemBase item, CancellationToken cancellationToken)
    {
        if (!await _itemValidator.ValidateAsync(item).ConfigureAwait(false))
        {
            return false;
        }

        _items2.Add(item);

        return true;
    }

    public Task<bool> EditItemAsync(ItemBase item, CancellationToken cancellationToken)
    {
        var index = _items2.IndexOf(item);

        if (index == -1)
        {
            return Task.FromResult(false);
        }
        _items2.RemoveAt(index);
        _items2.Insert(index, item);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteItemAsync(ItemBase item, CancellationToken cancellationToken)
    {
        var index = _items2.IndexOf(item);

        if (index == -1)
        {
            return Task.FromResult(false);
        }

        _items2.RemoveAt(index);

        return Task.FromResult(true);
    }

    public async ValueTask<IReadOnlyList<ItemBase>> GetItemsAsync(CancellationToken cancellationToken)
    {
        if (_items2.Count == 0)
        {
            await LoadItemsAsync(cancellationToken).ConfigureAwait(false);
        }

        return _items2;
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

    private static bool TryGetItem(string line, [NotNullWhen(true)] out ItemBase? item)
    {
        var infos = line.Split(CsvSeparator);
        var name = infos[0].Trim();
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
            item = default;
            return false;
        }

        switch (type)
        {
            case ItemType.Weapon:
                item = new Weapon(
                    name,
                    subtype,
                    status: infos[3].Trim(),
                    statusChancePercentage,
                    status2: infos[5].Trim(),
                    status2ChancePercentage,
                    requiredStrength,
                    requiredDexterity,
                    requiredIntelligence,
                    minDamage,
                    maxDamage,
                    speed,
                    sockets);
                break;

            case ItemType.Offhand:
                item = new Offhand(
                    name: name,
                    subtype,
                    requiredStrength,
                    requiredDexterity,
                    minDefense,
                    maxDefense,
                    minBlock,
                    maxBlock,
                    totalMinBlock: infos[18],
                    totalMaxBlock: infos[19],
                    sockets);
                break;
            case ItemType.Armor:
                item = new Armor(name, subtype, requiredStrength, requiredDexterity, requiredIntelligence, minDefense, maxDefense, sockets);
                break;

            case ItemType.Jewelry:
                item = new Jewelry(name, subtype, sockets);
                break;

            case ItemType.MagicWeapon:
            case ItemType.MeleeWeapon:
            case ItemType.RangeWeapon:
            default:
                item = default;
                return false;
        }

        return true;
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
                if (TryGetItem(line, out var item))
                {
                    _items2.Add(item);
                }
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to load items");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation("{NumberOfItems} items loaded in {ElapsedTimeInMillisecond} ms", _items2.Count, stopwatch.ElapsedMilliseconds);
        }
    }
}
