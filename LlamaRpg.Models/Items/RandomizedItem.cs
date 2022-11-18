namespace LlamaRpg.Models.Items;

public sealed class RandomizedItem
{
    private readonly List<string> _affixes = new();
    private readonly List<string> _baseAffixes = new();

    public RandomizedItem(
        int index,
        string itemName,
        ItemType itemType,
        ItemSubtype itemSubtype,
        int powerLevel,
        ItemRarityType itemRarityType)
    {
        Index = index;
        ItemName = itemName;
        ItemType = itemType;
        ItemSubtype = itemSubtype;
        PowerLevel = powerLevel;
        ItemRarityType = itemRarityType;
    }

    public int Index { get; }
    public string ItemName { get; }
    public ItemType ItemType { get; }
    public ItemSubtype ItemSubtype { get; }
    public int PowerLevel { get; }
    public IReadOnlyList<string> BaseAffixes => _baseAffixes;
    public IReadOnlyList<string> Affixes => _affixes;
    public ItemRarityType ItemRarityType { get; }

    public void SetAffixes(IReadOnlyList<string> baseAffixes, IReadOnlyList<string> affixes)
    {
        foreach (var baseAffix in baseAffixes)
        {
            _baseAffixes.Add(baseAffix);
        }

        foreach (var affix in affixes)
        {
            _affixes.Add(affix);
        }
    }
}
