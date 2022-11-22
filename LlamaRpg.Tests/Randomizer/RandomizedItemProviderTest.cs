using LlamaRpg.Models.Affixes;
using LlamaRpg.Models.Items;
using LlamaRpg.Models.Items.PrimaryTypes;
using LlamaRpg.Models.Randomizer;
using LlamaRpg.Services.Randomization;
using LlamaRpg.Services.Readers;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace LlamaRpg.Tests.Randomizer;

public class RandomizedItemProviderTest
{
    private readonly RandomizedItemProvider _provider;

    private readonly IReadOnlyList<ItemBase> _items = CreateSampleItems();

    public RandomizedItemProviderTest()
    {
        var itemProvider = Substitute.For<IItemProvider>();

        itemProvider.GetItemsAsync(Arg.Any<CancellationToken>()).Returns(_items);

        var affixProvider = Substitute.For<IAffixProvider>();

        affixProvider.GetAffixesAsync(Arg.Any<CancellationToken>()).Returns(Array.Empty<Affix>());

        var randomizedAffixProvider = Substitute.For<IRandomizedAffixProvider>();

        randomizedAffixProvider
            .GenerateAffixes(
                Arg.Any<ItemBase>(),
                Arg.Any<int>(),
                Arg.Any<ItemRarityType>(),
                Arg.Any<IEnumerable<Affix>>(),
                Arg.Any<int>(),
                Arg.Any<ItemNumberOfAffixes>())
            .Returns((Array.Empty<string>(), Array.Empty<string>()));

        _provider = new RandomizedItemProvider(itemProvider, affixProvider, randomizedAffixProvider, NullLogger<RandomizedItemProvider>.Instance);
    }

    [Theory]
    [InlineData(ItemType.Weapon)]
    [InlineData(ItemType.Offhand)]
    [InlineData(ItemType.Armor)]
    public async void GenerateItems_WithOneSpecificItemType_CorrectlyGenerateThatItemType(ItemType itemType)
    {
        // Arrange
        var settings = new ItemRandomizerSettings(
            1,
            10,
            ItemDropRates.Default(),
            ItemNumberOfAffixes.Default(),
            new List<ItemTypeWeightDrop> { new(itemType, 1) });

        // Act
        var generatedItems = new List<RandomizedItem>();

        await foreach (var item in _provider.GenerateItemsAsync(settings, CancellationToken.None))
        {
            generatedItems.Add(item);
        }

        // Assert
        Assert.Single(generatedItems);
        Assert.Equal(itemType, generatedItems[0].ItemType);
    }

    private static IReadOnlyList<ItemBase> CreateSampleItems()
    {
        var items = new List<ItemBase>
        {
            new Weapon("War Axe", ItemSubtype.Axe, ItemType.MeleeWeapon, string.Empty, 0, string.Empty, 0, 1, 0, 0, 1, 2, 0, 0, 10, 0),
            new Weapon("Long Sword", ItemSubtype.Sword, ItemType.MeleeWeapon, string.Empty, 0, string.Empty, 0, 1, 0, 0, 1, 2, 0, 0, 10, 0),
            new Offhand("Small Shield", ItemSubtype.Shield, 2, 0, 1, 2, 1, 2, string.Empty, string.Empty, 0),
            new Armor("Quilted Armor", ItemSubtype.Chest, 3, 0, 0, 1, 5, 2),
        };

        return items.AsReadOnly();
    }
}
