namespace RpgFilesGeneratorTools.Models.ItemTypes;

internal sealed class Jewelry : ItemBase
{
    public Jewelry(
        string name,
        ItemSubtype subtype,
        int sockets
    )
        : base(name, subtype, 0, 0, 0, sockets)
    {
        Type = ItemType.Jewelry;
    }
}
