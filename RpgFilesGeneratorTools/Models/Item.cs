namespace RpgFilesGeneratorTools.Models;

internal sealed class Item
{
    public Item(
        string name,
        ItemType type,
        ItemSubtype subtype,
        string status,
        int statusChance,
        string status2,
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
        string totalMinBlock,
        string totalMaxBlock,
        string totalMin,
        string totalMax,
        int speed,
        int sockets)
    {
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

    public string Name { get; set; }
    public ItemType Type { get; set; }
    public ItemSubtype Subtype { get; set; }
    public string Status { get; set; }
    public int StatusChance { get; set; }
    public string Status2 { get; set; }
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
    public string TotalMinBlock { get; set; }
    public string TotalMaxBlock { get; set; }
    public string TotalMin { get; set; }
    public string TotalMax { get; set; }
    public int Speed { get; set; }
    public int Sockets { get; set; }

    public Item Clone()
    {
        return new Item(
            Name,
            Type,
            Subtype,
            Status,
            StatusChance,
            Status2,
            Status2Chance,
            RequiredStrength,
            RequiredDexterity,
            RequiredIntelligence,
            MinDamage,
            MaxDamage,
            MinDefense,
            MaxDefense,
            MinBlock,
            MaxBlock,
            TotalMinBlock,
            TotalMaxBlock,
            TotalMin,
            TotalMax,
            Speed,
            Sockets);
    }
}
