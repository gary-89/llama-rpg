namespace LlamaRpg.Models.Items;

public sealed record ItemCountPerType(ItemType ItemType, int Count);

public sealed record ItemCountPerPowerLevel(int PowerLevel, int Count, double Percentage)
{
    public string GetPowerLevelDisplayName() => ((PowerLevelItem)PowerLevel).ToString();
}

public enum PowerLevelItem
{
    Normal = 1,
    Exceptional = 2,
    Elite = 3,
    Upgrade = 4,
    PerfectUpgrade = 5,
}
