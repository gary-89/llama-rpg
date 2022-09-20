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
    int Speed);
