namespace LlamaRpg.Models;

public sealed class Range
{
    private int _min;
    private int _max;

    public Range(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public int Min
    {
        get => _min;
        set
        {
            _min = value;
            OnStatusChanged();
        }
    }

    public int Max
    {
        get => _max;
        set
        {
            _max = value;
            OnStatusChanged();
        }
    }

    public event EventHandler<EventArgs>? RangeChanged;

    private void OnStatusChanged()
    {
        RangeChanged?.Invoke(this, EventArgs.Empty);
    }
}
