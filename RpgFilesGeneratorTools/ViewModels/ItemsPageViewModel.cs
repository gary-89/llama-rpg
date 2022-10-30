using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using RpgFilesGeneratorTools.Models;
using RpgFilesGeneratorTools.Services;
using RpgFilesGeneratorTools.Toolkit.Async;
using RpgFilesGeneratorTools.Toolkit.Extensions;
using static System.String;

namespace RpgFilesGeneratorTools.ViewModels;

internal class ItemsPageViewModel : ObservableObject
{
    private readonly IItemProvider _itemProvider;
    private readonly ILogger<ItemsPageViewModel> _logger;

    private Item? _selectedItem;
    private string? _filterText;
    private IReadOnlyList<Item> _items = new List<Item>();
    private Item? _editingItem;
    private bool _isEditing;
    private bool _displayDetails;
    private int _selectedIndex = -1;

    public ItemsPageViewModel(IItemProvider itemProvider, ILogger<ItemsPageViewModel> logger)
    {
        _itemProvider = itemProvider;
        _logger = logger;

        EditCommand = new RelayCommand(() => IsEditing = true);
        CancelCommand = new RelayCommand(() => IsEditing = false);
        ClearSelectionCommand = new RelayCommand(ClearSelection);
        SaveCommand = new RelayCommand(SaveItem);
        DeleteItemStatusCommand = new RelayCommand<object>(DeleteItemStatus);

        TaskInitialize = new NotifyTaskCompletion<bool>(LoadItemsAsync());
    }

    private void DeleteItemStatus(object? statusIndex)
    {
        if (EditingItem is null || !int.TryParse(statusIndex?.ToString(), out var index))
        {
            return;
        }

        switch (index)
        {
            case 0:
                EditingItem.Status = Empty;
                EditingItem.StatusChance = default;
                OnPropertyChanged(nameof(EditingItem));
                break;

            case 1:
                EditingItem.Status2 = Empty;
                EditingItem.Status2Chance = default;
                OnPropertyChanged(nameof(EditingItem));
                break;
        }
    }

    public NotifyTaskCompletion<bool> TaskInitialize { get; }

    public ICommand ClearSelectionCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteItemStatusCommand { get; }

    public string? FilterText
    {
        get => _filterText;
        set
        {
            if (SetProperty(ref _filterText, value))
            {
                RefreshList();
            }
        }
    }

    public ObservableCollection<Item> ItemsSource { get; } = new();

    public IReadOnlyList<ItemType> ItemTypes { get; } = Enum.GetValues<ItemType>();

    public IReadOnlyList<ItemSubtype> ItemSubtypes { get; } = Enum.GetValues<ItemSubtype>();

    public bool DisplayDetails
    {
        get => _displayDetails;
        private set => SetProperty(ref _displayDetails, value);
    }

    public int SelectedIndex
    {
        get => _selectedIndex;
        set => SetProperty(ref _selectedIndex, value);
    }

    public Item? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (value is null)
            {
                DisplayDetails = false;
                return;
            }

            if (SetProperty(ref _selectedItem, value))
            {
                EditingItem = value.Clone();
            }

            DisplayDetails = true;
        }
    }

    public ObservableCollection<string> ItemStatusesSource { get; } = new();

    public Item? EditingItem
    {
        get => _editingItem;
        private set => SetProperty(ref _editingItem, value);
    }

    public bool IsEditing
    {
        get => _isEditing;
        private set => SetProperty(ref _isEditing, value);
    }

    private void ClearSelection()
    {
        DisplayDetails = false;
        SelectedIndex = -1;
    }

    private void SaveItem()
    {
        if (EditingItem is null)
        {
            IsEditing = false;
            return;
        }

        var editedItem = ItemsSource.FirstOrDefault(x => x.Name == EditingItem?.Name);

        if (editedItem is null)
        {
            IsEditing = false;
            return;
        }

        editedItem.Type = EditingItem.Type;
        editedItem.Subtype = EditingItem.Subtype;
        editedItem.Speed = EditingItem.Speed;
        editedItem.RequiredDexterity = EditingItem.RequiredDexterity;
        editedItem.RequiredStrength = EditingItem.RequiredStrength;
        editedItem.RequiredIntelligence = EditingItem.RequiredIntelligence;
        editedItem.MinDamage = EditingItem.MinDamage;
        editedItem.MaxDamage = EditingItem.MaxDamage;
        editedItem.MinBlock = EditingItem.MinBlock;
        editedItem.MaxBlock = EditingItem.MaxBlock;
        editedItem.MinDefense = EditingItem.MinDefense;
        editedItem.MaxDefense = EditingItem.MaxDefense;
        editedItem.Status = EditingItem.Status;
        editedItem.StatusChance = EditingItem.StatusChance;
        editedItem.Status2 = EditingItem.Status2;
        editedItem.Status2Chance = EditingItem.Status2Chance;

        OnPropertyChanged(nameof(SelectedItem));

        IsEditing = false;
    }

    private async Task<bool> LoadItemsAsync()
    {
        try
        {
            _items = await _itemProvider.GetItemsAsync(CancellationToken.None).ConfigureAwait(true);
            ItemsSource.AddEach(_items);

            var statuses = new HashSet<string>();

            foreach (var item in _items)
            {
                if (!IsNullOrEmpty(item.Status))
                {
                    statuses.Add(item.Status);
                }

                if (!IsNullOrEmpty(item.Status2))
                {
                    statuses.Add(item.Status2);
                }
            }

            ItemStatusesSource.AddEach(statuses);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to load items");
        }

        return true;
    }

    private void RefreshList()
    {
        ItemsSource.Clear();

        ItemsSource.AddEach(
            IsNullOrWhiteSpace(FilterText)
                ? _items
                : _items.Where(x => x.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                                    x.Type.ToString().Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                                    x.Subtype.ToString().Contains(FilterText, StringComparison.OrdinalIgnoreCase)));
    }
}
