using LlamaRpg.Models.Items;

namespace RpgFilesGeneratorTools.ViewModels.Randomizer;

internal sealed class ItemTypeWeightDrop
{
    public static int DefaultWeaponWeight = 3;
    public static int DefaultOffhandWeight = 2;
    public static int DefaultArmorWeight = 2;
    public static int DefaultJewelryWeight = 1;

    public ItemTypeWeightDrop(ItemType itemType, int weight)
    {
        ItemType = itemType;
        Weight = weight;
    }

    public ItemType ItemType { get; }
    public int Weight { get; set; }
}
