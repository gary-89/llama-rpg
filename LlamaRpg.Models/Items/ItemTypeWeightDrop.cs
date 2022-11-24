namespace LlamaRpg.Models.Items;

public sealed class ItemTypeWeightDrop
{
    public const int DefaultWeaponWeight = 3;
    public const int DefaultOffhandWeight = 2;
    public const int DefaultArmorWeight = 2;
    public const int DefaultJewelryWeight = 1;

    public ItemTypeWeightDrop(ItemType itemType, int weight)
    {
        ItemType = itemType;
        Weight = weight;
    }

    public ItemType ItemType { get; }

    public int Weight { get; set; }

    public static IReadOnlyList<ItemTypeWeightDrop> Default()
        => new List<ItemTypeWeightDrop>()
        {
            new(ItemType.Weapon, 2),
            new(ItemType.Armor, 1),
            new(ItemType.Offhand, 1),
            new(ItemType.Jewelry, 1),
        };
}
