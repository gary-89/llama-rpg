using LlamaRpg.Models.Affixes;
using LlamaRpg.Models.Items;

namespace LlamaRpg.Services.Randomization;

public interface IRandomizerAffixValidator
{
    bool ValidateRarity(AffixRule rule, ItemRarityType rarity);
    bool ValidateWeaponElements(Affix affix, SecondaryElement? secondaryElementOfWeapon);
}
