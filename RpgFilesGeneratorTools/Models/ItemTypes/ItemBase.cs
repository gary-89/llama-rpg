using System;

namespace RpgFilesGeneratorTools.Models.ItemTypes;

internal abstract class ItemBase : IEquatable<ItemBase>
{
    protected ItemBase(string name, ItemSubtype subtype, int requiredStrength, int requiredDexterity, int requiredIntelligence, int sockets)
    {
        Id = Guid.NewGuid();
        Name = name;
        Subtype = subtype;
        RequiredStrength = requiredStrength;
        RequiredDexterity = requiredDexterity;
        RequiredIntelligence = requiredIntelligence;
        Sockets = sockets;
    }

    public Guid Id { get; }
    public string Name { get; }
    public ItemType Type { get; protected set; }
    public ItemSubtype Subtype { get; }
    public int RequiredStrength { get; }
    public int RequiredDexterity { get; }
    public int RequiredIntelligence { get; }
    public int Sockets { get; }

    public bool Equals(ItemBase? other) => other is not null && Id.Equals(other.Id);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is ItemBase other && Equals(other);

    public override int GetHashCode() => Id.GetHashCode();
}
