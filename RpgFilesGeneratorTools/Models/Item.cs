namespace RpgFilesGeneratorTools.Models;

internal record Item(
    string Name,
    string Type,
    string SubType,
    string Status,
    int StatusChance,
    string Status2,
    int Status2Chance,
    int RequiredStrength,
    int RequiredDexterity,
    int MinDamage,
    int MaxDamage,
    int Speed)
{
    public static Item Empty() => new Item(
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
}
