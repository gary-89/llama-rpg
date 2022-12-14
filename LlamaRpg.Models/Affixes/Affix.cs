using System.ComponentModel.DataAnnotations;
using System.Reflection;
using LlamaRpg.Models.Items;

namespace LlamaRpg.Models.Affixes;

public sealed class Affix
{
    private const string CommaSeparator = ", ";
    private const string Damage = "damage";
    private const string Defense = "defense";

    private static readonly IReadOnlyList<string> PrimaryElements = Enum.GetNames(typeof(PrimaryElement));
    private static readonly IReadOnlyList<string> SecondaryElements = Enum.GetNames(typeof(SecondaryElement));

    private string? _types;

    public Affix(string name, bool hasPercentageModifier)
    {
        Name = name;
        HasPercentageModifier = hasPercentageModifier;

        SetType();
        SetPrimaryElement();
        SetSecondaryElement();
    }

    public string Name { get; }

    public AffixAttributeType Attribute { get; private set; }

    public bool HasPercentageModifier { get; }

    public AffixType Type { get; private set; }

    public PrimaryElement? PrimaryElement { get; private set; }

    public SecondaryElement? SecondaryElement { get; private set; }

    public List<AffixRule> Rules { get; } = new();

    public void SetAttribute(AffixAttributeType affixAttributeType)
    {
        Attribute = affixAttributeType;
    }

    public string GetItemTypes()
    {
        return _types ??= string.Join(
            CommaSeparator,
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

    private void SetPrimaryElement()
    {
        foreach (var primaryElementString in PrimaryElements)
        {
            if (!Name.Contains(primaryElementString, StringComparison.OrdinalIgnoreCase) ||
                !Enum.TryParse<PrimaryElement>(primaryElementString, out var primaryElement))
            {
                continue;
            }

            PrimaryElement = primaryElement;
            break;
        }
    }

    private void SetSecondaryElement()
    {
        foreach (var secondaryElementString in SecondaryElements)
        {
            if (!Name.Contains(secondaryElementString, StringComparison.OrdinalIgnoreCase) ||
                !Enum.TryParse<SecondaryElement>(secondaryElementString, out var secondaryElement))
            {
                continue;
            }

            SecondaryElement = secondaryElement;
            break;
        }
    }

    private void SetType()
    {
        if (Name.Contains(Damage, StringComparison.OrdinalIgnoreCase))
        {
            Type = AffixType.ElementalDamage;
        }
        else if (Name.Contains(Defense, StringComparison.OrdinalIgnoreCase))
        {
            Type = Name.Equals(Defense, StringComparison.OrdinalIgnoreCase)
                ? AffixType.Defense
                : AffixType.ElementalDefense;
        }
        else
        {
            Type = AffixType.Undefined;
        }
    }
}
