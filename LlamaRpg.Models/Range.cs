namespace LlamaRpg.Models;

public class RangeChangedEventArgs : EventArgs
{
    public int OldValue { get; }
    public int NewValue { get; }

    public RangeChangedEventArgs(int oldValue, int newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

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
            OnStatusChanged(value, Max);
        }
    }

    public int Max
    {
        get => _max;
        set
        {
            _max = value;
            OnStatusChanged(Min, value);
        }
    }

    public event EventHandler<RangeChangedEventArgs>? RangeChanged;

    private void OnStatusChanged(int min, int max)
    {
        RangeChanged?.Invoke(this, new RangeChangedEventArgs(min, max));
    }
}
