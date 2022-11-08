namespace LlamaRpg.App.ViewModels.Randomizer;

internal sealed class Range
{
    public Range(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public int Min { get; set; }
    public int Max { get; set; }
}
