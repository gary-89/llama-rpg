namespace RpgFilesGeneratorTools.ViewModels;

internal sealed class ItemTypeWeightDrop
{
    public ItemTypeWeightDrop(string itemType, int weight)
    {
        ItemType = itemType;
        Weight = weight;
    }

    public string ItemType { get; }
    public int Weight { get; set; }
}
