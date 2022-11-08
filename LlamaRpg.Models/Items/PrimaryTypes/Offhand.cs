namespace LlamaRpg.Models.Items.PrimaryTypes;

public sealed class Offhand : ItemBase
{
    public Offhand(
        string name,
        ItemSubtype subtype,
        int requiredStrength,
        int requiredDexterity,
        int minDefense,
        int maxDefense,
        int minBlock,
        int maxBlock,
        string? totalMinBlock,
        string? totalMaxBlock,
        int sockets
    )
        : base(name, subtype, requiredStrength, requiredDexterity, requiredIntelligence: 0, sockets)
    {
        Type = ItemType.Offhand;
        MinDefense = minDefense;
        MaxDefense = maxDefense;
        MinBlock = minBlock;
        MaxBlock = maxBlock;
        TotalMinBlock = totalMinBlock;
        TotalMaxBlock = totalMaxBlock;
    }

    public int MinDefense { get; }
    public int MaxDefense { get; }
    public int MinBlock { get; }
    public int MaxBlock { get; }
    public string? TotalMinBlock { get; }
    public string? TotalMaxBlock { get; }
}
