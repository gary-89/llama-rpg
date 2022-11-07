using RpgFilesGeneratorTools.Models;

namespace RpgFilesGeneratorTools.Services;

internal interface IItemRandomizerValidator
{
    bool ValidateRarity(AffixRule rule, ItemRarityType rarity);
}
