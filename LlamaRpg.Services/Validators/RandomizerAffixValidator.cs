using LlamaRpg.Models.Affixes;
using LlamaRpg.Models.Items;

namespace LlamaRpg.Services.Validators;

internal sealed class RandomizerAffixValidator : IRandomizerAffixValidator
{
    private const string Enhance = "enhanced";

    public bool ValidateRarity(AffixRule rule, ItemRarityType rarity)
    {
        if (rarity == ItemRarityType.Normal)
        {
            return true;
        }

        if (rule.IsEliteOnly && rarity == ItemRarityType.Elite)
        {
            return true;
        }

        if (rule.IsRare && rarity is ItemRarityType.Magic or ItemRarityType.Rare)
        {
            return true;
        }

        if (rule.IsElite && rarity is ItemRarityType.Magic or ItemRarityType.Rare or ItemRarityType.Elite)
        {
            return true;
        }

        return rarity == ItemRarityType.Magic && !rule.IsEliteOnly;
    }

    public bool ValidateItemElements(Affix affix, PrimaryElement? primaryElementOfWeapon, SecondaryElement? secondaryElementOfWeapon)
    {
        return (primaryElementOfWeapon.HasValue == false || ValidatePrimaryElementOfItem(affix, primaryElementOfWeapon.Value))
               && (secondaryElementOfWeapon.HasValue == false || ValidateSecondaryElementOfItem(affix, secondaryElementOfWeapon.Value));
    }

    public bool ValidatePrimaryElementOfItem(Affix affix, PrimaryElement primaryElementOfWeapon)
    {
        if (affix.PrimaryElement.HasValue == false
            || affix.Name.Contains(Enhance, StringComparison.OrdinalIgnoreCase) == false)
        {
            return true;
        }

        return affix.PrimaryElement.Value == primaryElementOfWeapon;
    }

    public bool ValidateSecondaryElementOfItem(Affix affix, SecondaryElement secondaryElementOfWeapon)
    {
        if (affix.PrimaryElement.HasValue == false
            || affix.Name.Contains(Enhance, StringComparison.OrdinalIgnoreCase) == false)
        {
            return true;
        }

        return affix.PrimaryElement.Value switch
        {
            PrimaryElement.Cold => secondaryElementOfWeapon is SecondaryElement.Ice or SecondaryElement.Water,
            PrimaryElement.Fire => secondaryElementOfWeapon is SecondaryElement.Burn or SecondaryElement.Heat,
            PrimaryElement.Electric => secondaryElementOfWeapon is SecondaryElement.Spark or SecondaryElement.Lightning,
            PrimaryElement.Poison => secondaryElementOfWeapon is SecondaryElement.Venom or SecondaryElement.Acid,
            _ => throw new ArgumentOutOfRangeException(nameof(secondaryElementOfWeapon))
        };
    }
}
