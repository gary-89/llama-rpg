namespace LlamaRpg.Models.Randomizer;

public sealed record ItemDropRates(int MagicItemDropRate, int RareItemDropRate, int EliteItemDropRate)
{
    public static ItemDropRates Default() => new(1, 3, 5);
}
