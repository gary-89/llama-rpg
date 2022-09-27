using System;
using System.Collections.Generic;
using System.IO;
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

    public ItemProvider(AppConfig appConfig, ILogger<ItemProvider> logger)
    {
        _appConfig = appConfig;
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
            var itemsFilePath = Path.Combine(_appConfig.AssetsFilesFolder, "Items.csv");
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

        //name,0
        //Type,1
        //Subtype,2
        //Status,3
        //StatusChance,4
        //Status2,5
        //StatusChance2,6
        //reqstr,7
        //reqdex,8
        //reqint,9
        //Sockets,10
        //speed,11
        //mindam,12
        //maxdam,13
        //mindef,
        //maxdef,minblock,maxblock,Total min block,Total max block,Total min dmg/def,Total max dmg/def,Socketed,level (null for now)

        int.TryParse(infos[4].Replace("%", ""), out var statusChancePercentage);
        int.TryParse(infos[6].Replace("%", ""), out var status2ChancePercentage);
        int.TryParse(infos[7], out var requiredStrength);
        int.TryParse(infos[8], out var requiredDexterity);
        int.TryParse(infos[12], out var minDamage);
        int.TryParse(infos[13], out var maxDamage);
        int.TryParse(infos[11], out var speed);

        var item = new Item(
            name: infos[0],
            type: infos[1],
            subtype: infos[2],
            status: infos[3],
            statusChance: statusChancePercentage,
            status2: infos[5],
            status2Chance: status2ChancePercentage,
            requiredStrength: requiredStrength,
            requiredDexterity: requiredDexterity,
            minDamage: minDamage,
            maxDamage: maxDamage,
            speed: speed);

        _items.Add(item);
    }
}
