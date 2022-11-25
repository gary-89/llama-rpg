using LlamaRpg.Models.Affixes;
using LlamaRpg.Models.Items;

namespace LlamaRpg.Services.Validators;

public interface IRandomizerAffixValidator
{
    bool ValidateRarity(AffixRule rule, ItemRarityType rarity);
    bool ValidateItemElements(Affix affix, PrimaryElement? primaryElementOfWeapon, SecondaryElement? secondaryElementOfWeapon);
    bool ValidateRule(AffixRule rule, ItemType itemType, ItemSubtype itemSubtype, int settingsMonsterLevel, int itemPowerLevel, ItemRarityType itemRarity);
}
