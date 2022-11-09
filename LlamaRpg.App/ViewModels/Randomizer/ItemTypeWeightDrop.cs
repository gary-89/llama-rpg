using LlamaRpg.Models.Items;

namespace RpgFilesGeneratorTools.ViewModels.Randomizer;

internal sealed class ItemTypeWeightDrop
{
    public ItemTypeWeightDrop(ItemType itemType, int weight)
    {
        ItemType = itemType;
        Weight = weight;
    }

    public ItemType ItemType { get; }
    public int Weight { get; set; }
}
