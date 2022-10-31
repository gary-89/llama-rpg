﻿using System.Collections.Generic;
using System.Linq;
using RpgFilesGeneratorTools.Toolkit.Extensions;

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
        return _types ??= string.Join(CommaSeparator,
            Rules.SelectMany(x => x.ItemTypes).Distinct().Select(GetEnumDisplayName)
                .Concat(Rules.SelectMany(x => x.ItemSubtypes).Distinct().Select(GetEnumDisplayName)));
    }

    public IEnumerable<ItemTypeAffixes> GetPerItemTypeAffixes()
    {
        var rulesByItemType = new Dictionary<string, List<AffixRule>>();

        // TODO: try to use LINQ statements
        foreach (var rule in Rules)
        {
            foreach (var ruleItemType in rule.ItemTypes)
            {
                var key = ruleItemType.GetDisplayName();
                if (!rulesByItemType.ContainsKey(key))
                {
                    rulesByItemType.Add(key, new List<AffixRule>());
                }
                rulesByItemType[key].Add(rule);
            }

            foreach (var ruleItemType in rule.ItemSubtypes)
            {
                var key = ruleItemType.GetDisplayName();
                if (!rulesByItemType.ContainsKey(key))
                {
                    rulesByItemType.Add(key, new List<AffixRule>());
                }
                rulesByItemType[key].Add(rule);
            }
        }

        return rulesByItemType.Select(x => new ItemTypeAffixes(x.Key, x.Value));
    }

    private static string GetEnumDisplayName(ItemType enumValue) => enumValue.GetDisplayName();
    private static string GetEnumDisplayName(ItemSubtype enumValue) => enumValue.GetDisplayName();
}
