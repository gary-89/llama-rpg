namespace RpgFilesGeneratorTools.Models;

internal sealed class Item
{
    public Item(
        string name,
        string type,
        string subtype,
        string status,
        int statusChance,
        string status2,
        int status2Chance,
        int requiredStrength,
        int requiredDexterity,
        int minDamage,
        int maxDamage,
        int speed)
    {
        this.Name = name;
        this.Type = type;
        this.Subtype = subtype;
        this.Status = status;
        this.StatusChance = statusChance;
        this.Status2 = status2;
        this.Status2Chance = status2Chance;
        this.RequiredStrength = requiredStrength;
        this.RequiredDexterity = requiredDexterity;
        this.MinDamage = minDamage;
        this.MaxDamage = maxDamage;
        this.Speed = speed;
    }

    public static Item Empty() => new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        0,
        string.Empty,
        0,
        0,
        0,
        0,
        0,
        0);

    public string Name { get; set; }
    public string Type { get; set; }
    public string Subtype { get; set; }
    public string Status { get; set; }
    public int StatusChance { get; set; }
    public string Status2 { get; set; }
    public int Status2Chance { get; set; }
    public int RequiredStrength { get; set; }
    public int RequiredDexterity { get; set; }
    public int MinDamage { get; set; }
    public int MaxDamage { get; set; }
    public int Speed { get; set; }

    public Item Clone()
    {
        return new Item(Name, Type, Subtype, Status, StatusChance, Status2, Status2Chance, RequiredStrength, RequiredDexterity, MinDamage, MaxDamage, Speed);
    }
}
