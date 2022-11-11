using LlamaRpg.Models.Affixes;
using LlamaRpg.Models.Items;

namespace LlamaRpg.Services.Randomization;

public sealed class RandomizerAffixValidator : IRandomizerAffixValidator
{
    public bool ValidateRarity(AffixRule rule, ItemRarityType rarity)
    {
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

    public bool ValidateWeaponElements(Affix affix, SecondaryElement? secondaryElementOfWeapon)
    {
        if (secondaryElementOfWeapon.HasValue == false || (affix.PrimaryElement is null && affix.SecondaryElement is null))
        {
            return true;
        }

        return affix.SecondaryElement.HasValue
            ? secondaryElementOfWeapon.Value == affix.SecondaryElement.Value
            : ValidateElement(secondaryElementOfWeapon.Value, affix.PrimaryElement);
    }

    private static bool ValidateElement(SecondaryElement secondaryElementOfWeapon, PrimaryElement? affixPrimaryElement)
    {
        if (affixPrimaryElement.HasValue == false)
        {
            return true;
        }

        return affixPrimaryElement.Value switch
        {
            PrimaryElement.Cold => secondaryElementOfWeapon is SecondaryElement.Ice or SecondaryElement.Water,
            PrimaryElement.Fire => secondaryElementOfWeapon is SecondaryElement.Burn or SecondaryElement.Heat,
            PrimaryElement.Lightning => secondaryElementOfWeapon is SecondaryElement.Spark or SecondaryElement.Electric,
            PrimaryElement.Poison => secondaryElementOfWeapon is SecondaryElement.Venom or SecondaryElement.Acid,
            _ => throw new ArgumentOutOfRangeException(nameof(affixPrimaryElement))
        };
    }
}
