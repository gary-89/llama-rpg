using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LlamaRpg.App.Toolkit.Extensions;
using LlamaRpg.Models.Items;

namespace LlamaRpg.App.Models;

internal sealed class ItemViewModel : ObservableObject
{
    private string _name = string.Empty;
    private ItemType _type;
    private ItemSubtype? _subtype;

    public ItemViewModel(string name, ItemType type, ItemSubtype subtype)
    {
        Id = Guid.NewGuid();
        Name = name;
        Type = type;
        Subtype = subtype;
    }

    public ItemViewModel(
        Guid id,
        string name,
        ItemType type,
        ItemSubtype subtype,
        string? status,
        int statusChance,
        string? status2,
        int status2Chance,
        int requiredStrength,
        int requiredDexterity,
        int requiredIntelligence,
        int minDamage,
        int maxDamage,
        int minDefense,
        int maxDefense,
        int minBlock,
        int maxBlock,
        string? totalMinBlock,
        string? totalMaxBlock,
        string? totalMin,
        string? totalMax,
        int speed,
        int sockets)
    {
        Id = id;
        Name = name;
        Type = type;
        Subtype = subtype;
        Status = status;
        StatusChance = statusChance;
        Status2 = status2;
        Status2Chance = status2Chance;
        RequiredStrength = requiredStrength;
        RequiredDexterity = requiredDexterity;
        RequiredIntelligence = requiredIntelligence;
        MinDamage = minDamage;
        MaxDamage = maxDamage;
        MinDefense = minDefense;
        MaxDefense = maxDefense;
        MinBlock = minBlock;
        MaxBlock = maxBlock;
        TotalMinBlock = totalMinBlock;
        TotalMaxBlock = totalMaxBlock;
        TotalMin = totalMin;
        TotalMax = totalMax;
        Speed = speed;
        Sockets = sockets;
    }

    public ItemViewModel(Item item) :
        this(
            item.Id,
            item.Name,
            item.Type,
            item.Subtype,
            item.Status,
            item.StatusChance,
            item.Status2,
            item.Status2Chance,
            item.RequiredStrength,
            item.RequiredDexterity,
            item.RequiredIntelligence,
            item.MinDamage,
            item.MaxDamage,
            item.MinDefense,
            item.MaxDefense,
            item.MinBlock,
            item.MaxBlock,
            item.TotalMinBlock,
            item.TotalMaxBlock,
            item.TotalMin,
            item.TotalMax,
            item.Speed,
            item.Sockets)
    {
    }

    public ObservableCollection<ItemSubtype> ItemSubtypes { get; } = new()
    {
        ItemSubtype.Axe,
        ItemSubtype.Mace,
        ItemSubtype.Sword,
        ItemSubtype.Bow,
        ItemSubtype.Crossbow,
        ItemSubtype.Staff,
        ItemSubtype.Wand
    };

    public Guid Id { get; set; }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public ItemType Type
    {
        get => _type;
        set
        {
            if (!SetProperty(ref _type, value))
            {
                return;
            }

            switch (value)
            {
                case ItemType.Weapon:
                    Subtype = default;
                    ItemSubtypes.Clear();
                    ItemSubtypes.AddEach(new[]
                    {
                        ItemSubtype.Axe, ItemSubtype.Mace, ItemSubtype.Sword, ItemSubtype.Bow,
                        ItemSubtype.Crossbow, ItemSubtype.Staff, ItemSubtype.Wand
                    });
                    Subtype = ItemSubtype.Axe;
                    break;

                case ItemType.Armor:
                    Subtype = default;
                    ItemSubtypes.Clear();
                    ItemSubtypes.AddEach(new[] { ItemSubtype.Chest, ItemSubtype.Boots });
                    Subtype = ItemSubtype.Chest;
                    break;

                case ItemType.Jewelry:
                    Subtype = default;
                    ItemSubtypes.Clear();
                    ItemSubtypes.AddEach(new[] { ItemSubtype.Necklace });
                    Subtype = ItemSubtype.Necklace;
                    break;

                case ItemType.Offhand:
                    Subtype = default;
                    ItemSubtypes.Clear();
                    ItemSubtypes.AddEach(new[] { ItemSubtype.Shield });
                    Subtype = ItemSubtype.Shield;
                    break;
            }
        }
    }

    public ItemSubtype? Subtype
    {
        get => _subtype;
        set => SetProperty(ref _subtype, value);
    }

    public string? Status { get; set; }
    public int StatusChance { get; set; }
    public string? Status2 { get; set; }
    public int Status2Chance { get; set; }
    public int RequiredStrength { get; set; }
    public int RequiredDexterity { get; set; }
    public int RequiredIntelligence { get; set; }
    public int MinDamage { get; set; }
    public int MaxDamage { get; set; }
    public int MinDefense { get; set; }
    public int MaxDefense { get; set; }
    public int MinBlock { get; set; }
    public int MaxBlock { get; set; }
    public string? TotalMinBlock { get; set; }
    public string? TotalMaxBlock { get; set; }
    public string? TotalMin { get; set; }
    public string? TotalMax { get; set; }
    public int Speed { get; set; }
    public int Sockets { get; set; }
}
