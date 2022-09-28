namespace RpgFilesGeneratorTools.ViewModels;

internal sealed class ItemTypeFrequencyDrop
{
    public ItemTypeFrequencyDrop(string name, int frequency)
    {
        Name = name;
        Frequency = frequency;
    }

    public string Name { get; set; }
    public int Frequency { get; set; }
}
