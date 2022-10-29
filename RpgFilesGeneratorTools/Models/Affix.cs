using System.Collections.Generic;
using System.Linq;

namespace RpgFilesGeneratorTools.Models;

internal sealed class Affix
{
    private const string CommaSeparator = ", ";
    private string? _types;

    public Affix(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public List<AffixRule> Rules { get; } = new();

    public string GetItemTypes()
    {
        return _types ??= string.Join(CommaSeparator, Rules.SelectMany(x => x.ItemTypes).Distinct());
    }

    public IEnumerable<ItemTypeAffixes> GetPerItemTypeAffixes()
    {
        var rulesByItemType = new Dictionary<string, List<AffixRule>>();

        // TODO: try to use LINQ statements
        foreach (var rule in Rules)
        {
            foreach (var ruleItemType in rule.ItemTypes)
            {
                if (!rulesByItemType.ContainsKey(ruleItemType))
                {
                    rulesByItemType.Add(ruleItemType, new List<AffixRule>());
                }
                rulesByItemType[ruleItemType].Add(rule);
            }
        }

        return rulesByItemType.Select(x => new ItemTypeAffixes(x.Key, x.Value));
    }
}
