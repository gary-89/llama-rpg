using System.Collections.Generic;
using System.Linq;

namespace RpgFilesGeneratorTools.Models;

internal sealed record AffixDetails(string Name, List<AffixItemType> Affixes);

internal sealed class Affix
{
    private readonly Dictionary<string, List<AffixItemType>> _affixesByItemType = new();

    private string? _types;

    public Affix(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public List<AffixItemType> ItemTypes { get; } = new();

    public string GetTypes()
    {
        return _types ??= string.Join(", ", ItemTypes.Select(x => x.Type).Distinct());
    }

    public IEnumerable<AffixDetails> GetTypesEnumerable() => ItemTypes.GroupBy(x => x.Type).Select(x => new AffixDetails(x.Key, x.ToList()));

    public IReadOnlyDictionary<string, List<AffixItemType>> GetByItemType()
    {
        if (_affixesByItemType.Keys.Count != 0)
        {
            return _affixesByItemType;
        }

        foreach (var affixItemType in ItemTypes)
        {
            if (!_affixesByItemType.ContainsKey(affixItemType.Type))
            {
                _affixesByItemType.Add(affixItemType.Type, new List<AffixItemType>());
            }

            _affixesByItemType[affixItemType.Type].Add(affixItemType);
        }

        return _affixesByItemType;
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
