using LlamaRpg.Models.Affixes;
using LlamaRpg.Models.Items;
using LlamaRpg.Models.Items.PrimaryTypes;

namespace LlamaRpg.Services.Validators;

public interface IRandomizerAffixValidator
{
    bool ValidateRarity(AffixRule rule, ItemRarityType rarity);
    bool ValidateItemElements(Affix affix, PrimaryElement? primaryElementOfWeapon, SecondaryElement? secondaryElementOfWeapon);
    bool ValidateRule(AffixRule affixRule, ItemBase item, int settingsMonsterLevel, int itemPowerLevel);
}
