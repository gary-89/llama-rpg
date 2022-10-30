using System.Collections.Generic;

namespace RpgFilesGeneratorTools.Models;

internal enum ItemType
{
    Weapon,
    Offhand,
    Armor,
    Jewelry,
    MeleeWeapon,
    MagicWeapon,
    RangeWeapon,
};

internal enum ItemSubtype
{
    Sword,
    Axe,
    Mace,
    Bow,
    Crossbow,
    Staff,
    Wand,
    Shield,
    Boots,
    Chest,
    Necklace,
};

internal sealed record ItemTypeAffixes(ItemType ItemType, List<AffixRule> Affixes);
