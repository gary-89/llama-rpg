using LlamaRpg.Models.Affixes;
using LlamaRpg.Models.Items;

namespace LlamaRpg.Services.Validators;

internal sealed class RandomizerAffixValidator : IRandomizerAffixValidator
{
    private const string Enhance = "enhanced";

    public bool ValidateRarity(AffixRule rule, ItemRarityType rarity)
    {
        return rarity switch
        {
            ItemRarityType.Normal or ItemRarityType.Magic => rule.IsEliteOnly == false,
            ItemRarityType.Rare => rule.IsEliteOnly == false && rule.IsRare,
            ItemRarityType.Elite => rule.IsElite,
            _ => throw new InvalidOperationException("Invalid item rarity")
        };
    }

    public bool ValidateRule(AffixRule r, ItemType itemType, ItemSubtype itemSubtype, int itemLevelRequired, int itemPowerLevelRequired)
    {
        return r.ItemLevelRequired < itemLevelRequired
               && r.PowerLevelRequired <= itemPowerLevelRequired
               && (r.ItemTypes.Contains(itemType)
                   || r.ItemSubtypes.Contains(itemSubtype)
                   || (r.ItemTypes.Contains(ItemType.MeleeWeapon) && (itemSubtype is ItemSubtype.Axe or ItemSubtype.Sword or ItemSubtype.Mace))
                   || (r.ItemTypes.Contains(ItemType.RangeWeapon) && (itemSubtype is ItemSubtype.Bow or ItemSubtype.Crossbow))
                   || (r.ItemTypes.Contains(ItemType.MagicWeapon) && (itemSubtype is ItemSubtype.Wand or ItemSubtype.Staff)));
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
