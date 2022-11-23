using System.Runtime.CompilerServices;
using LlamaRpg.Models.Items;
using LlamaRpg.Models.Monsters;
using LlamaRpg.Models.Randomizer;
using LlamaRpg.Services.Readers;

namespace LlamaRpg.Services.Randomization;

internal sealed class RandomizedMonsterProvider : IRandomizedMonsterProvider
{
    private readonly IMonsterProvider _monsterProvider;
    private readonly IRandomizedItemProvider _randomizedItemProvider;
    private readonly Random _random = new();

    public RandomizedMonsterProvider(IMonsterProvider monsterProvider, IRandomizedItemProvider randomizedItemProvider)
    {
        _monsterProvider = monsterProvider;
        _randomizedItemProvider = randomizedItemProvider;
    }

    public async IAsyncEnumerable<RandomizedMonster> GenerateMonstersAsync(
        MonsterRandomizerSettings settings,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var monsters = await _monsterProvider.GetMonsterAsync(cancellationToken).ConfigureAwait(false);

        for (var i = 0; i < settings.NumberOfMonstersToGenerate; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var source = monsters.Where(x => (settings.MonsterAreaType is null || x.MonsterArea == settings.MonsterAreaType)
                                             && x.Level >= settings.MonsterLevel.Min
                                             && x.Level <= settings.MonsterLevel.Max).ToList();

            var index = _random.Next(0, source.Count);

            var monster = source[index];

            var uniqueBoss = _random.Next(0, 100);

            var uniqueMonsterType = uniqueBoss > 30
                ? UniqueMonsterType.Normal
                : uniqueBoss > 10
                    ? UniqueMonsterType.Boss
                    : UniqueMonsterType.SuperBoss;

            var itemSettings = new ItemRandomizerSettings(
                NumberOfItemsToGenerate: 8,
                monster.Level,
                DropRateSettings.Default(),
                NumberOfAffixesSettings.Default(),
                new[]
                {
                    new ItemTypeWeightDrop(ItemType.MagicWeapon, 1),
                    new ItemTypeWeightDrop(ItemType.MeleeWeapon, 1),
                    new ItemTypeWeightDrop(ItemType.MagicWeapon, 1),
                    new ItemTypeWeightDrop(ItemType.Offhand, 1),
                });

            var items = new List<RandomizedItem>();

            await foreach (var item in _randomizedItemProvider.GenerateItemsAsync(itemSettings, cancellationToken))
            {
                items.Add(item);
            }

            var monsterLevel = monster.Level + uniqueMonsterType switch
            {
                UniqueMonsterType.Boss => 5,
                UniqueMonsterType.SuperBoss => 10,
                _ => 0
            };

            var generatedMonster = new RandomizedMonster(
                i + 1,
                monster.Name,
                monster.Type,
                uniqueMonsterType,
                monster.MonsterArea,
                monsterLevel,
                items);

            yield return generatedMonster;
        }
    }
}
