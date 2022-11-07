using LlamaRpg.Models.Items;

namespace LlamaRpg.Models.Models.ItemTypes;

public sealed class Armor : ItemBase
{
    public Armor(
        string name,
        ItemSubtype subtype,
        int requiredStrength,
        int requiredDexterity,
        int requiredIntelligence,
        int minDefense,
        int maxDefense,
        int sockets
    )
        : base(name, subtype, requiredStrength, requiredDexterity, requiredIntelligence, sockets)
    {
        Type = ItemType.Armor;
        MinDefense = minDefense;
        MaxDefense = maxDefense;
    }

    public int MinDefense { get; set; }
    public int MaxDefense { get; set; }
}
