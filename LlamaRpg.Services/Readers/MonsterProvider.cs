using LlamaRpg.Models.Monsters;
using Microsoft.Extensions.Logging;

namespace LlamaRpg.Services.Readers;

internal sealed class MonsterProvider : IMonsterProvider
{
    private const char CsvSeparator = ',';
    private const string MonstersFileName = "Monsters.csv";

    private readonly AppServicesConfig _appServicesConfig;
    private readonly List<Monster> _monsters = new();
    private readonly ILogger<MonsterProvider> _logger;

    public MonsterProvider(AppServicesConfig appServicesConfig, ILogger<MonsterProvider> logger)
    {
        _appServicesConfig = appServicesConfig;
        _logger = logger;
    }

    public async ValueTask<IReadOnlyList<Monster>> GetMonsterAsync(CancellationToken cancellationToken)
    {
        if (_monsters.Count == 0)
        {
            await LoadAsync(cancellationToken).ConfigureAwait(false);
        }

        return _monsters.AsReadOnly();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        try
        {
            var itemsFilePath = Path.Combine(_appServicesConfig.AssetsFilesFolder, MonstersFileName);

            var lines = await File.ReadAllLinesAsync(itemsFilePath, cancellationToken).ConfigureAwait(false);

            foreach (var line in lines[1..])
            {
                var infos = line.Split(CsvSeparator);

                var areaTypeString = infos[0];
                var monsterName = infos[1];
                var monsterType = infos[2];
                var monsterLevelString = infos[4];

                if (string.IsNullOrWhiteSpace(monsterName)
                    || string.IsNullOrWhiteSpace(monsterType)
                    || !int.TryParse(areaTypeString, out var areaTypeInt)
                    || !int.TryParse(monsterLevelString, out var monsterLevel))
                {
                    continue; // Invalid monster: skip
                }

                var monster = new Monster(monsterName, monsterType, (MonsterAreaType)areaTypeInt, monsterLevel);

                _monsters.Add(monster);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Load affixes failed");
            throw;
        }
    }
}
