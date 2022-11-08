using LlamaRpg.Models.Affixes;
using LlamaRpg.Models.Items;
using LlamaRpg.Services.Randomization;

namespace LlamaRpg.Tests.Randomizer;

public class RandomizerAffixValidatorTest
{
    private readonly RandomizerAffixValidator _validator;

    public RandomizerAffixValidatorTest()
    {
        _validator = new RandomizerAffixValidator();
    }

    [Theory]
    [InlineData(ItemRarityType.Elite, true, true, false, true)]
    [InlineData(ItemRarityType.Elite, true, false, false, false)]
    [InlineData(ItemRarityType.Elite, false, false, false, false)]
    [InlineData(ItemRarityType.Elite, false, false, true, true)]
    [InlineData(ItemRarityType.Rare, true, true, false, true)]
    [InlineData(ItemRarityType.Rare, true, false, false, true)]
    [InlineData(ItemRarityType.Rare, false, false, false, false)]
    [InlineData(ItemRarityType.Rare, false, false, true, false)]
    [InlineData(ItemRarityType.Magic, true, true, false, true)]
    [InlineData(ItemRarityType.Magic, true, false, false, true)]
    [InlineData(ItemRarityType.Magic, false, false, false, true)]
    [InlineData(ItemRarityType.Magic, false, false, true, false)]
    public void ValidateRarity_ForDifferentRarities_CorrectlyCheckTheAffixRule(
        ItemRarityType rarity,
        bool isRare,
        bool isElite,
        bool isOnlyElite,
        bool expectedResult)
    {
        // Arrange
        var rule = CreateAffixRule(isRare, isElite, isOnlyElite);

        // Act
        var result = _validator.ValidateRarity(rule, rarity);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    private static AffixRule CreateAffixRule(bool isRare, bool isElite, bool isOnlyElite)
    {
        return new AffixRule(
            new[] { ItemType.Armor },
            new[] { ItemSubtype.Axe },
            Tier: 1,
            IsRare: isRare,
            IsElite: isElite,
            IsEliteOnly: isOnlyElite,
            0,
            0,
            0,
            0,
            0,
            "Mod",
            "0",
            "0",
            AffixModifierType.Number,
            0,
            0,
            AffixVariance.FixedNumber);
    }
}
