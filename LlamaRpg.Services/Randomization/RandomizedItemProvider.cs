using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using LlamaRpg.Models.Items;
using LlamaRpg.Models.Randomizer;
using LlamaRpg.Services.Readers;
using Microsoft.Extensions.Logging;

namespace LlamaRpg.Services.Randomization;

internal sealed class RandomizedItemProvider : IRandomizedItemProvider
{
    private readonly IItemProvider _itemProvider;
    private readonly IAffixProvider _affixProvider;
    private readonly IRandomizedAffixProvider _randomAffixProvider;
    private readonly ILogger<RandomizedItemProvider> _logger;

    private readonly Random _random = new();

    public RandomizedItemProvider(
        IItemProvider itemProvider,
        IAffixProvider affixProvider,
        IRandomizedAffixProvider randomAffixProvider,
        ILogger<RandomizedItemProvider> logger)
    {
        _itemProvider = itemProvider;
        _affixProvider = affixProvider;
        _randomAffixProvider = randomAffixProvider;
        _logger = logger;
    }

    public async IAsyncEnumerable<RandomizedItem> GenerateItemsAsync(
        ItemRandomizerSettings settings,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var items = await _itemProvider.GetItemsAsync(cancellationToken).ConfigureAwait(false);
        var affixes = await _affixProvider.GetAffixesAsync(cancellationToken).ConfigureAwait(false);

        // TODO: handle scenario when one item must be of a specific type (ex: only one weapon)
        var totalWeights = settings.ItemTypeWeights.Sum(x => x.Weight) + 1;
        var cumulativeWeights = new List<ItemTypeCumulativeWeight>(settings.ItemTypeWeights.Count);
        var cumulativeWeight = 0;

        foreach (var itemTypeWeight in settings.ItemTypeWeights)
        {
            cumulativeWeight += itemTypeWeight.Weight;
            cumulativeWeights.Add(new ItemTypeCumulativeWeight(itemTypeWeight.ItemType, cumulativeWeight));
        }

        var counter = 1;

        for (var i = 0; i < settings.NumberOfItemsToGenerate; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var randomType = _random.Next(1, totalWeights);

            var itemType = cumulativeWeights.First(x => x.CumulativeWeight >= randomType).ItemType;

            var possibleItems = items.Where(x => x.Type == itemType || x.SubType2 == itemType).ToList();

            if (!possibleItems.Any())
            {
                throw new InvalidOperationException("No item available: impossible to generate randomized items.");
            }

            var item = possibleItems.ElementAt(_random.Next(possibleItems.Count));

            if (!TryGenerateRandomizedItem(ref counter, item.Name, item.Type, item.Subtype, settings, out var randomItem))
            {
                continue;
            }

            try
            {
                var (baseAffixes, generatedAffixes) = _randomAffixProvider.GenerateAffixes(
                    item,
                    randomItem.PowerLevel,
                    randomItem.ItemRarityType,
                    affixes,
                    settings.MonsterLevel,
                    settings.ItemNumberOfAffixes);

                randomItem.SetAffixes(baseAffixes, generatedAffixes);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Something when wrong during the affix generation");
                continue;
            }

            yield return randomItem;
        }
    }

    private bool TryGenerateRandomizedItem(
        ref int counter,
        string itemName,
        ItemType itemType,
        ItemSubtype itemSubtype,
        ItemRandomizerSettings settings,
        [NotNullWhen(true)] out RandomizedItem? result)
    {
        try
        {
            var rarity = GenerateRarity(settings, itemType == ItemType.Jewelry);

            var powerLevel = itemType == ItemType.Jewelry ? 0 : GenerateItemPowerLevel(settings.MonsterLevel);

            result = new RandomizedItem(counter++, itemName, itemType, itemSubtype, powerLevel, rarity);

            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to generate a random drop: {Message}", exception.Message);
            result = default;
            return false;
        }
    }

    private int GenerateItemPowerLevel(int monsterLevel)
    {
        const int defaultChanceOfDropMaxPowerLevelItem = 30;

        while (true)
        {
            if (monsterLevel < 25)
            {
                return 1;
            }

            if (_random.Next(1, 100) <= defaultChanceOfDropMaxPowerLevelItem)
            {
                return monsterLevel switch
                {
                    < 50 => 2,
                    _ => 3
                };
            }

            monsterLevel -= 25;
        }
    }

    private ItemRarityType GenerateRarity(ItemRandomizerSettings settings, bool isJewelry)
    {
        var rarity = _random.Next(0, settings.ItemDropRates.EliteItemDropRate * settings.ItemDropRates.RareItemDropRate * settings.ItemDropRates.MagicItemDropRate);

        return rarity % settings.ItemDropRates.EliteItemDropRate == 0
            ? ItemRarityType.Elite
            : rarity % settings.ItemDropRates.RareItemDropRate == 0
                ? ItemRarityType.Rare
                : rarity % settings.ItemDropRates.MagicItemDropRate == 0
                    ? ItemRarityType.Magic
                    : isJewelry
                        ? ItemRarityType.Magic
                        : ItemRarityType.Normal;
    }

    private sealed record ItemTypeCumulativeWeight(ItemType ItemType, int CumulativeWeight);
}
