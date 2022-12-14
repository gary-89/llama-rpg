using LlamaRpg.Models.Affixes;
using LlamaRpg.Models.Items;
using Microsoft.Extensions.Logging;

namespace LlamaRpg.Services.Readers;

internal sealed class AffixProvider : IAffixProvider
{
    private const char CsvSeparator = ',';

    private const string EmptySpace = " ";
    private const string AffixesFileName = "Affixes.csv";
    private const string MinDamage = "mindam";
    private const string MaxDamagePerPowerLevel = "maxdam * plvl";
    private const string MinBlock = "minblock";
    private const string MaxBlockPerPowerLevel = "maxblock * plvl";
    private const string PowerLevelPlusMinBlock = "plvl + minblock";
    private const string PowerLevelTimesThreePlusMaxBlock = "plvl * 3 + maxblock";

    private readonly AppServicesConfig _appServicesConfig;
    private readonly List<Affix> _affixes = new();
    private readonly ILogger<AffixProvider> _logger;

    private readonly HashSet<string> _percentageBaseAffixNames = new()
    {
        "Accuracy",
        "Enhanced",
        "Critical Strike",
        "Life Steal",
        "Mana Steal",
        "Reflect",
        "Resistance",
        "Avoidance",
        "Evasion",
        "Find",
    };

    public AffixProvider(AppServicesConfig appServicesConfig, ILogger<AffixProvider> logger)
    {
        _appServicesConfig = appServicesConfig;
        _logger = logger;
    }

    public async ValueTask<IReadOnlyList<Affix>> GetAffixesAsync(CancellationToken cancellationToken)
    {
        if (_affixes.Count == 0)
        {
            await LoadAsync(cancellationToken).ConfigureAwait(false);
        }

        return _affixes.AsReadOnly();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        try
        {
            var itemsFilePath = Path.Combine(_appServicesConfig.AssetsFilesFolder, AffixesFileName);

            var lines = await File.ReadAllLinesAsync(itemsFilePath, cancellationToken).ConfigureAwait(false);

            var index = -1;

            foreach (var line in lines[1..])
            {
                var infos = line.Split(CsvSeparator);
                var affixName = infos[0].Trim();

                if (!string.IsNullOrWhiteSpace(affixName))
                {
                    var hasPercentageModifier = _percentageBaseAffixNames.Any(x =>
                        affixName.Contains(x, StringComparison.OrdinalIgnoreCase) ||
                        x.Contains(affixName, StringComparison.OrdinalIgnoreCase));

                    var affix = new Affix(
                        affixName,
                        hasPercentageModifier);

                    _affixes.Add(affix);
                    index++;
                    continue;
                }

                var itemTypeString = infos[21].Trim().Replace(EmptySpace, string.Empty);
                var itemTypeString2 = infos[22].Trim().Replace(EmptySpace, string.Empty);
                var itemTypeString3 = infos[23].Trim().Replace(EmptySpace, string.Empty);
                var itemTypeString4 = infos[24].Trim().Replace(EmptySpace, string.Empty);
                var itemTypeString5 = infos[25].Trim().Replace(EmptySpace, string.Empty);
                var itemTypeString6 = infos[26].Trim().Replace(EmptySpace, string.Empty);

                if (string.IsNullOrWhiteSpace(infos[1]) ||
                    (!Enum.TryParse<ItemType>(itemTypeString, true, out _) && !Enum.TryParse<ItemSubtype>(itemTypeString, true, out _)) ||
                    !int.TryParse(infos[1], out var tier))
                {
                    continue; // Invalid affix: skip
                }

                var affixAttributeTypeString = infos[6].Trim();

                if (!Enum.TryParse<AffixAttributeType>(affixAttributeTypeString, true, out var affixAttributeType))
                {
                    _logger.LogError("Invalid attribute type: cannot define if affix is a prefix or suffix.");
                    continue; // Invalid affix: skip
                }

                List<ItemType> itemTypes = new();
                List<ItemSubtype> itemSubtypes = new();

                foreach (var value in new[] { itemTypeString, itemTypeString2, itemTypeString3, itemTypeString4, itemTypeString5, itemTypeString6 }
                             .Where(t => !string.IsNullOrWhiteSpace(t)))
                {
                    if (Enum.TryParse<ItemType>(value, true, out var itemType))
                    {
                        itemTypes.Add(itemType);
                    }
                    else if (Enum.TryParse<ItemSubtype>(value, true, out var itemSubtype))
                    {
                        itemSubtypes.Add(itemSubtype);
                    }
                }

                var lastAffix = _affixes[index];
                lastAffix.SetAttribute(affixAttributeType);
                lastAffix.Rules.Add(CreateAffixRule(itemTypes, itemSubtypes, tier, infos));
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Load affixes failed");
            throw;
        }
    }

    private static AffixRule CreateAffixRule(IReadOnlyList<ItemType> itemTypes, IReadOnlyList<ItemSubtype> itemSubtypes, int tier, IReadOnlyList<string> infos)
    {
        int.TryParse(infos[2], out var isRare);
        int.TryParse(infos[3], out var isElite);
        int.TryParse(infos[4], out var isEliteOnly);
        int.TryParse(infos[5], out var itemLevelRequired);
        int.TryParse(infos[7], out var powerLevelRequired);
        int.TryParse(infos[8], out var maxLevel);
        int.TryParse(infos[9], out var frequency);
        int.TryParse(infos[10], out var group);
        int.TryParse(infos[20], out var varianceNumber);

        var variance = (AffixVariance)varianceNumber;

        var modifier1Text = infos[11].Trim();
        var modifier1MinText = infos[12].Trim();
        var modifier1MaxText = infos[13].Trim();

        var affixModifierType = AffixModifierType.Undefined;
        var modifier1Max = 0;

        if (int.TryParse(modifier1MinText, out var modifier1Min) && int.TryParse(modifier1MaxText, out modifier1Max))
        {
            affixModifierType = AffixModifierType.Number;
        }
        else if (modifier1MinText.Contains(MinDamage, StringComparison.OrdinalIgnoreCase) &&
                modifier1MaxText.Equals(MaxDamagePerPowerLevel, StringComparison.OrdinalIgnoreCase))
        {
            affixModifierType = AffixModifierType.MinimumDamagePlus;

            int.TryParse(modifier1MinText.Replace($"{MinDamage} + ", string.Empty), out modifier1Min);
        }
        else if (modifier1MinText.Contains(MinBlock, StringComparison.OrdinalIgnoreCase) &&
                modifier1MaxText.Equals(MaxBlockPerPowerLevel, StringComparison.OrdinalIgnoreCase))
        {
            affixModifierType = AffixModifierType.MinimumBlockPlus;

            int.TryParse(modifier1MinText.Replace($"{MinBlock} + ", string.Empty), out modifier1Min);
        }
        else if (modifier1MinText.Equals(PowerLevelPlusMinBlock, StringComparison.OrdinalIgnoreCase) &&
                 modifier1MaxText.Equals(PowerLevelTimesThreePlusMaxBlock, StringComparison.OrdinalIgnoreCase))
        {
            affixModifierType = AffixModifierType.PowerLevelPlusMinimumBlock;
        }

        return new AffixRule(
            itemTypes,
            itemSubtypes,
            tier,
            isRare == 1,
            isElite == 1,
            isEliteOnly == 1,
            itemLevelRequired,
            powerLevelRequired,
            group,
            maxLevel,
            frequency,
            modifier1Text,
            modifier1MinText,
            modifier1MaxText,
            affixModifierType,
            modifier1Min,
            modifier1Max,
            variance);
    }
}
