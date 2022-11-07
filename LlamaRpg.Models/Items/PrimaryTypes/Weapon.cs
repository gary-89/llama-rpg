using LlamaRpg.Models.Items;

namespace LlamaRpg.Models.Models.ItemTypes;

public sealed class Weapon : ItemBase
{
    public Weapon(
        string name,
        ItemSubtype subtype,
        string? status,
        int statusChance,
        string? status2,
        int status2Chance,
        int requiredStrength,
        int requiredDexterity,
        int requiredIntelligence,
        int minDamage,
        int maxDamage,
        int speed,
        int sockets
    )
        : base(name, subtype, requiredStrength, requiredDexterity, requiredIntelligence, sockets)
    {
        Type = ItemType.Weapon;
        Speed = speed;
        Status = status;
        Status2 = status2;
        StatusChance = statusChance;
        Status2Chance = status2Chance;
        MinDamage = minDamage;
        MaxDamage = maxDamage;
    }

    public string? Status { get; }
    public string? Status2 { get; }
    public int StatusChance { get; }
    public int Status2Chance { get; }
    public int MinDamage { get; }
    public int MaxDamage { get; }
    public int Speed { get; }
}
