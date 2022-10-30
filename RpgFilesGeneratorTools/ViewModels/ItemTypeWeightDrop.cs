using RpgFilesGeneratorTools.Models;

namespace RpgFilesGeneratorTools.ViewModels;

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
