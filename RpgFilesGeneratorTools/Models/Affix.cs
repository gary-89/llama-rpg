using System.Collections.Generic;
using System.Linq;

namespace RpgFilesGeneratorTools.Models;

internal sealed class Affix
{
    public Affix(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public List<AffixItemType> ItemTypes { get; } = new();

    public string GetTypes()
    {
        return string.Join(", ", ItemTypes.Select(x => x.Type).Distinct());
    }
}

internal sealed class AffixItemType
{
    public AffixItemType(string type, int tier, bool isRare, bool isElite, int itemLevel, int frequency, string modifier1, string modifier1Min, string modifier1Max)
    {
        Type = type;
        Tier = tier;
        IsRare = isRare;
        IsElite = isElite;
        ItemLevel = itemLevel;
        Frequency = frequency;
        Modifier1 = modifier1;
        Modifier1Min = modifier1Min;
        Modifier1Max = modifier1Max;
    }

    public string Type { get; set; }
    public int Tier { get; set; }
    public bool IsRare { get; set; }
    public bool IsElite { get; set; }
    public int ItemLevel { get; set; }
    public int Frequency { get; set; }
    public string Modifier1 { get; set; }
    public string Modifier1Min { get; set; }
    public string Modifier1Max { get; set; }
}
