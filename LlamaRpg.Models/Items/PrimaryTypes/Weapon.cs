namespace LlamaRpg.Models.Items.PrimaryTypes;

public sealed class Weapon : ItemBase
{
    public Weapon(
        string name,
        ItemSubtype subtype,
        ItemType? subtype2,
        string? status,
        int statusChance,
        string? status2,
        int status2Chance,
        int requiredStrength,
        int requiredDexterity,
        int requiredIntelligence,
        int minDamage,
        int maxDamage,
        int minBlock,
        int maxBlock,
        int speed,
        int sockets
    )
        : base(name, subtype, requiredStrength, requiredDexterity, requiredIntelligence, sockets)
    {
        Type = ItemType.Weapon;
        SubType2 = subtype2;
        Speed = speed;
        Status = status;
        Status2 = status2;
        StatusChance = statusChance;
        Status2Chance = status2Chance;
        MinDamage = minDamage;
        MaxDamage = maxDamage;
        MinBlock = minBlock;
        MaxBlock = maxBlock;
    }

    public string? Status { get; }
    public string? Status2 { get; }
    public int StatusChance { get; }
    public int Status2Chance { get; }
    public int MinDamage { get; }
    public int MaxDamage { get; }
    public int MinBlock { get; }
    public int MaxBlock { get; }
    public int Speed { get; }
}
