namespace RpgFilesGeneratorTools.Models;

internal sealed record ItemCountPerType(ItemType ItemType, int Count);

internal sealed record ItemCountPerPowerLevel(int PowerLevel, int Count, double Percentage)
{
    public PowerLevelItem GetPowerLevelEnum() => (PowerLevelItem)PowerLevel;
}

internal enum PowerLevelItem
{
    Normal = 1,
    Exceptional = 2,
    Elite = 3,
    Upgrade = 4,
    PerfectUpgrade = 5,
}
