using LlamaRpg.Models.Affixes;
using LlamaRpg.Models.Items;

namespace LlamaRpg.Services.Randomization;

public interface IRandomizerAffixValidator
{
    bool ValidateRarity(AffixRule rule, ItemRarityType rarity);
}

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
}
