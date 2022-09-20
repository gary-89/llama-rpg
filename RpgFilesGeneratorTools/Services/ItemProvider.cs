using Microsoft.Extensions.Logging;
using RpgFilesGeneratorTools.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RpgFilesGeneratorTools.Services;

internal class ItemProvider : IItemProvider
{
    private const char CsvSeparator = ',';

    private readonly ILogger<ItemProvider> _logger;
    private readonly List<Item> _items = new();

    public ItemProvider(ILogger<ItemProvider> logger)
    {
        _logger = logger;
    }

    public async Task<IReadOnlyList<Item>> GetItemsAsync(CancellationToken cancellationToken)
    {
        if (_items.Count == 0)
        {
            await LoadItemsAsync(cancellationToken);
        }

        return _items;
    }

    private async Task LoadItemsAsync(CancellationToken cancellationToken)
    {

        try
        {
            var appPath = AppContext.BaseDirectory;
            var itemsFilePath = Path.Combine(appPath, "Assets", "Files", "Items.csv");
            var lines = await File.ReadAllLinesAsync(itemsFilePath, cancellationToken);

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
    }

    private void AddItem(string line)
    {
        var infos = line.Split(CsvSeparator);

        int.TryParse(infos[4].Replace("%", ""), out var statusChancePercentage);
        int.TryParse(infos[6].Replace("%", ""), out var status2ChancePercentage);
        int.TryParse(infos[7], out var requiredStrength);
        int.TryParse(infos[8], out var requiredDexterity);
        int.TryParse(infos[9], out var minDamage);
        int.TryParse(infos[10], out var maxDamage);
        int.TryParse(infos[11], out var speed);

        var item = new Item(
            Name: infos[0],
            Type: infos[1],
            SubType: infos[2],
            Status: infos[3],
            StatusChance: statusChancePercentage,
            Status2: infos[5],
            Status2Chance: status2ChancePercentage,
            RequiredStrength: requiredStrength,
            RequiredDexterity: requiredDexterity,
            MinDamage: minDamage,
            MaxDamage: maxDamage,
            Speed: speed);

        _items.Add(item);
    }
}
