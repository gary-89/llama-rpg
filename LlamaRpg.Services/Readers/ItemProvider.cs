using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using LlamaRpg.Models.Items;
using LlamaRpg.Models.Items.PrimaryTypes;
using LlamaRpg.Services.Validators;
using Microsoft.Extensions.Logging;

namespace LlamaRpg.Services.Readers;

internal sealed class ItemProvider : IItemProvider
{
    private const char CsvSeparator = ',';
    private const string Space = " ";



    private readonly AppServicesConfig _appServicesConfig;
    private readonly ItemValidator _itemValidator;
    private readonly ILogger<ItemProvider> _logger;
    private readonly List<ItemBase> _items2 = new();

    private List<ItemType>? _itemTypes;

    public ItemProvider(AppServicesConfig appServicesConfig, ItemValidator itemValidator, ILogger<ItemProvider> logger)
    {
        _appServicesConfig = appServicesConfig;
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
        int.TryParse(infos[5].Replace("%", ""), out var statusChancePercentage);
        int.TryParse(infos[7].Replace("%", ""), out var status2ChancePercentage);
        int.TryParse(infos[8].Replace("plvl *", ""), out var requiredStrength);
        int.TryParse(infos[9].Replace("plvl *", ""), out var requiredDexterity);
        int.TryParse(infos[10].Replace("plvl *", ""), out var requiredIntelligence);
        int.TryParse(infos[11], out var sockets);
        int.TryParse(infos[12], out var speed);
        int.TryParse(infos[13], out var minDamage);
        int.TryParse(infos[14], out var maxDamage);
        int.TryParse(infos[15], out var minDefense);
        int.TryParse(infos[16], out var maxDefense);
        int.TryParse(infos[17], out var minBlock);
        int.TryParse(infos[18], out var maxBlock);

        if (!Enum.TryParse<ItemType>(infos[1].Trim(), true, out var type) ||
            !Enum.TryParse<ItemSubtype>(infos[3].Trim(), true, out var subtype))
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
                    Enum.TryParse<ItemType>(infos[2].Replace(Space, string.Empty), true, out var subtype2) ? subtype2 : null,
                    status: infos[4].Trim(),
                    statusChancePercentage,
                    status2: infos[6].Trim(),
                    status2ChancePercentage,
                    requiredStrength,
                    requiredDexterity,
                    requiredIntelligence,
                    minDamage,
                    maxDamage,
                    minBlock,
                    maxBlock,
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
                    totalMinBlock: infos[19],
                    totalMaxBlock: infos[20],
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

            var itemsFilePath = Path.Combine(_appServicesConfig.AssetsFilesFolder, "Items.csv");
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
