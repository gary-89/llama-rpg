using System.ComponentModel.DataAnnotations;

namespace LlamaRpg.Models.Items;

public enum ItemType
{
    Weapon,

    Offhand,

    Armor,

    Jewelry,

    [Display(Name = "Melee weapon")]
    MeleeWeapon,

    [Display(Name = "Magic weapon")]
    MagicWeapon,

    [Display(Name = "Range weapon")]
    RangeWeapon,
};
