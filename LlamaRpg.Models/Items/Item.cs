namespace LlamaRpg.Models.Items;

public sealed class Item : IEquatable<Item>
{
    public Item(
        Guid id,
        string name,
        ItemType type,
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
        int minDefense,
        int maxDefense,
        int minBlock,
        int maxBlock,
        string? totalMinBlock,
        string? totalMaxBlock,
        string? totalMin,
        string? totalMax,
        int speed,
        int sockets)
    {
        Id = id;
        Name = name;
        Type = type;
        Subtype = subtype;
        Status = status;
        StatusChance = statusChance;
        Status2 = status2;
        Status2Chance = status2Chance;
        RequiredStrength = requiredStrength;
        RequiredDexterity = requiredDexterity;
        RequiredIntelligence = requiredIntelligence;
        MinDamage = minDamage;
        MaxDamage = maxDamage;
        MinDefense = minDefense;
        MaxDefense = maxDefense;
        MinBlock = minBlock;
        MaxBlock = maxBlock;
        TotalMinBlock = totalMinBlock;
        TotalMaxBlock = totalMaxBlock;
        TotalMin = totalMin;
        TotalMax = totalMax;
        Speed = speed;
        Sockets = sockets;
    }

    public Guid Id { get; }
    public string Name { get; set; }
    public ItemType Type { get; set; }
    public ItemSubtype Subtype { get; set; }
    public string? Status { get; set; }
    public int StatusChance { get; set; }
    public string? Status2 { get; set; }
    public int Status2Chance { get; set; }
    public int RequiredStrength { get; set; }
    public int RequiredDexterity { get; set; }
    public int RequiredIntelligence { get; set; }
    public int MinDamage { get; set; }
    public int MaxDamage { get; set; }
    public int MinDefense { get; set; }
    public int MaxDefense { get; set; }
    public int MinBlock { get; set; }
    public int MaxBlock { get; set; }
    public string? TotalMinBlock { get; set; }
    public string? TotalMaxBlock { get; set; }
    public string? TotalMin { get; set; }
    public string? TotalMax { get; set; }
    public int Speed { get; set; }
    public int Sockets { get; set; }

    public bool Equals(Item? other) => other is not null && Id.Equals(other.Id);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Item other && Equals(other);

    public override int GetHashCode() => Id.GetHashCode();
}
