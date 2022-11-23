namespace LlamaRpg.Models.Randomizer;

public sealed record DropRateSettings(int MagicItemDropRate, int RareItemDropRate, int EliteItemDropRate)
{
    public static DropRateSettings Default() => new(1, 3, 5);
}
