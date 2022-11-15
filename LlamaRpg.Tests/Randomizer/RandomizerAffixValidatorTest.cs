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
    [InlineData(ItemRarityType.Normal, false, false, false, true)]
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

    [Theory]
    [InlineData("Enhanced fire", SecondaryElement.Heat)]
    [InlineData("Enhanced fire", SecondaryElement.Burn)]
    [InlineData("Enhanced cold", SecondaryElement.Ice)]
    [InlineData("Enhanced cold", SecondaryElement.Water)]
    [InlineData("Enhanced electric", SecondaryElement.Spark)]
    [InlineData("Enhanced electric", SecondaryElement.Lightning)]
    [InlineData("Enhanced poison", SecondaryElement.Venom)]
    [InlineData("Enhanced poison", SecondaryElement.Acid)]
    public void ValidateEnhanceDamage_WithCorrectElementCorrelation_ReturnTrue(string affixName, SecondaryElement elementOfWeapon)
    {
        // Arrange
        var affix = new Affix(affixName, hasPercentageSuffix: false);

        // Act
        var result = _validator.ValidateEnhanceDamageAffix(affix, elementOfWeapon);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("Enhanced cold", SecondaryElement.Heat)]
    [InlineData("Enhanced poison", SecondaryElement.Heat)]
    [InlineData("Enhanced electric", SecondaryElement.Heat)]
    [InlineData("Enhanced cold", SecondaryElement.Burn)]
    [InlineData("Enhanced poison", SecondaryElement.Burn)]
    [InlineData("Enhanced electric", SecondaryElement.Burn)]
    [InlineData("Enhanced fire", SecondaryElement.Ice)]
    [InlineData("Enhanced electric", SecondaryElement.Ice)]
    [InlineData("Enhanced poison", SecondaryElement.Ice)]
    [InlineData("Enhanced fire", SecondaryElement.Water)]
    [InlineData("Enhanced electric", SecondaryElement.Water)]
    [InlineData("Enhanced poison", SecondaryElement.Water)]
    [InlineData("Enhanced fire", SecondaryElement.Spark)]
    [InlineData("Enhanced cold", SecondaryElement.Spark)]
    [InlineData("Enhanced poison", SecondaryElement.Spark)]
    [InlineData("Enhanced fire", SecondaryElement.Lightning)]
    [InlineData("Enhanced cold", SecondaryElement.Lightning)]
    [InlineData("Enhanced poison", SecondaryElement.Lightning)]
    [InlineData("Enhanced cold", SecondaryElement.Acid)]
    [InlineData("Enhanced fire", SecondaryElement.Acid)]
    [InlineData("Enhanced electric", SecondaryElement.Acid)]
    [InlineData("Enhanced cold", SecondaryElement.Venom)]
    [InlineData("Enhanced fire", SecondaryElement.Venom)]
    [InlineData("Enhanced electric", SecondaryElement.Venom)]
    public void ValidateEnhanceDamage_WithWrongElementCorrelation_ReturnFalse(string affixName, SecondaryElement elementOfWeapon)
    {
        // Arrange
        var affix = new Affix(affixName, hasPercentageSuffix: false);

        // Act
        var result = _validator.ValidateEnhanceDamageAffix(affix, elementOfWeapon);

        // Assert
        Assert.False(result);
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
