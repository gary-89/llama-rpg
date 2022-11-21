using LlamaRpg.Models.Affixes;
using LlamaRpg.Models.Items;
using LlamaRpg.Services.Validators;

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
    [InlineData(ItemRarityType.Normal, true, false, false, true)]
    [InlineData(ItemRarityType.Normal, true, true, false, true)]
    [InlineData(ItemRarityType.Normal, false, true, true, false)]
    [InlineData(ItemRarityType.Magic, false, false, false, true)]
    [InlineData(ItemRarityType.Magic, true, false, false, true)]
    [InlineData(ItemRarityType.Magic, true, true, false, true)]
    [InlineData(ItemRarityType.Magic, false, true, true, false)]
    [InlineData(ItemRarityType.Rare, false, false, false, false)]
    [InlineData(ItemRarityType.Rare, true, false, false, true)]
    [InlineData(ItemRarityType.Rare, true, true, false, true)]
    [InlineData(ItemRarityType.Rare, false, true, true, false)]
    [InlineData(ItemRarityType.Elite, false, false, false, false)]
    [InlineData(ItemRarityType.Elite, true, false, false, false)]
    [InlineData(ItemRarityType.Elite, true, true, false, true)]
    [InlineData(ItemRarityType.Elite, false, true, true, true)]
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
    public void ValidateItemSecondaryElement_WithCorrectElementCorrelation_ReturnTrue(string affixName, SecondaryElement elementOfItem)
    {
        // Arrange
        var affix = new Affix(affixName, hasPercentageModifier: false);

        // Act
        var result = _validator.ValidateSecondaryElementOfItem(affix, elementOfItem);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("Enhanced heat", PrimaryElement.Fire)]
    [InlineData("Enhanced burn", PrimaryElement.Fire)]
    [InlineData("Enhanced water", PrimaryElement.Cold)]
    [InlineData("Enhanced ice", PrimaryElement.Cold)]
    [InlineData("Enhanced spark", PrimaryElement.Electric)]
    [InlineData("Enhanced lightning", PrimaryElement.Electric)]
    [InlineData("Enhanced venom", PrimaryElement.Poison)]
    [InlineData("Enhanced acid", PrimaryElement.Poison)]
    public void ValidateItemPrimaryElement_WithCorrectElementCorrelation_ReturnTrue(string affixName, PrimaryElement elementOfItem)
    {
        // Arrange
        var affix = new Affix(affixName, hasPercentageModifier: false);

        // Act
        var result = _validator.ValidatePrimaryElementOfItem(affix, elementOfItem);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("Enhanced cold", PrimaryElement.Fire)]
    [InlineData("Enhanced poison", PrimaryElement.Fire)]
    [InlineData("Enhanced electric", PrimaryElement.Fire)]
    [InlineData("Enhanced fire", PrimaryElement.Cold)]
    [InlineData("Enhanced poison", PrimaryElement.Cold)]
    [InlineData("Enhanced electric", PrimaryElement.Cold)]
    [InlineData("Enhanced fire", PrimaryElement.Electric)]
    [InlineData("Enhanced cold", PrimaryElement.Electric)]
    [InlineData("Enhanced poison", PrimaryElement.Electric)]
    [InlineData("Enhanced fire", PrimaryElement.Poison)]
    [InlineData("Enhanced electric", PrimaryElement.Poison)]
    [InlineData("Enhanced cold", PrimaryElement.Poison)]
    public void ValidateItemPrimaryElement_WithWrongElementCorrelation_ReturnFalse(string affixName, PrimaryElement elementOfWeapon)
    {
        // Arrange
        var affix = new Affix(affixName, hasPercentageModifier: false);

        // Act
        var result = _validator.ValidatePrimaryElementOfItem(affix, elementOfWeapon);

        // Assert
        Assert.False(result);
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
    public void ValidateItemSecondaryElement_WithWrongElementCorrelation_ReturnFalse(string affixName, SecondaryElement elementOfWeapon)
    {
        // Arrange
        var affix = new Affix(affixName, hasPercentageModifier: false);

        // Act
        var result = _validator.ValidateSecondaryElementOfItem(affix, elementOfWeapon);

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
