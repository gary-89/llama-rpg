using System.ComponentModel.DataAnnotations;
using System.Reflection;
using LlamaRpg.Models.Items;

namespace LlamaRpg.Models.Affixes;

public sealed class Affix
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
        return _types ??= string.Join(CommaSeparator,
            Rules.SelectMany(x => x.ItemTypes).Distinct().Select(x => GetDisplayName(x))
                .Concat(Rules.SelectMany(x => x.ItemSubtypes).Distinct().Select(x => GetDisplayName(x))));
    }

    public IEnumerable<ItemTypeAffixes> GetPerItemTypeAffixes()
    {
        var rulesByItemType = new Dictionary<string, List<AffixRule>>();

        // TODO: try to use LINQ statements
        foreach (var rule in Rules)
        {
            foreach (var ruleItemType in rule.ItemTypes)
            {
                var key = GetDisplayName(ruleItemType);
                if (!rulesByItemType.ContainsKey(key))
                {
                    rulesByItemType.Add(key, new List<AffixRule>());
                }
                rulesByItemType[key].Add(rule);
            }

            foreach (var ruleItemType in rule.ItemSubtypes)
            {
                var key = GetDisplayName(ruleItemType);
                if (!rulesByItemType.ContainsKey(key))
                {
                    rulesByItemType.Add(key, new List<AffixRule>());
                }
                rulesByItemType[key].Add(rule);
            }
        }

        return rulesByItemType.Select(x => new ItemTypeAffixes(x.Key, x.Value));
    }

    private static string GetDisplayName(Enum enumValue)
    {
        var member = enumValue
            .GetType()
            .GetMember(enumValue.ToString())
            .FirstOrDefault();

        return member?.GetCustomAttribute<DisplayAttribute>()?.Name ?? enumValue.ToString();
    }
}
