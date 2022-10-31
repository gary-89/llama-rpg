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
    private readonly IList<ItemViewModel> _items = new List<ItemViewModel>();

    private ItemViewModel? _selectedItem;
    private string? _filterText;
    private ItemViewModel? _editingItem;
    private bool _isAdding;
    private bool _isEditing;
    private bool _displayDetails;
    private int _selectedIndex = -1;

    public ItemsPageViewModel(IItemProvider itemProvider, ILogger<ItemsPageViewModel> logger)
    {
        _itemProvider = itemProvider;
        _logger = logger;

        AddCommand = new RelayCommand(AddNewItem);
        EditCommand = new RelayCommand(() => IsEditing = true);
        CancelCommand = new RelayCommand(() =>
        {
            IsEditing = false;
            if (_isAdding)
            {
                DisplayDetails = false;
            }

            _isAdding = false;
        });
        ClearSelectionCommand = new RelayCommand(ClearSelection);
        SaveCommand = new AsyncRelayCommand(SaveItemAsync);
        DeleteItemStatusCommand = new RelayCommand<object>(DeleteItemStatus);

        TaskInitialize = new NotifyTaskCompletion<bool>(LoadItemsAsync());
    }

    public NotifyTaskCompletion<bool> TaskInitialize { get; }

    public ICommand ClearSelectionCommand { get; }
    public ICommand AddCommand { get; }
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

    public ObservableCollection<ItemViewModel> ItemsSource { get; } = new();

    public IReadOnlyList<ItemType> ItemTypes { get; private set; } = new List<ItemType>();

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

    public ItemViewModel? SelectedItem
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
                EditingItem = (ItemViewModel)value.Clone();
            }

            DisplayDetails = true;
        }
    }

    public ObservableCollection<string> ItemStatusesSource { get; } = new();

    public ItemViewModel? EditingItem
    {
        get => _editingItem;
        private set => SetProperty(ref _editingItem, value);
    }

    public bool IsEditing
    {
        get => _isEditing;
        private set => SetProperty(ref _isEditing, value);
    }

    private static Item CreateItem(ItemViewModel item)
    {
        var itemToAdd = new Item(item);
        return itemToAdd;
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

    private void AddNewItem()
    {
        _isAdding = true;
        IsEditing = true;
        DisplayDetails = true;
        EditingItem = new ItemViewModel("New item", ItemType.Weapon, ItemSubtype.Axe);
    }

    private void ClearSelection()
    {
        DisplayDetails = false;
        SelectedIndex = -1;
    }

    private async Task SaveItemAsync(CancellationToken cancellationToken)
    {
        if (EditingItem is null)
        {
            IsEditing = false;
            return;
        }

        if (_isAdding)
        {
            await SaveAddedItemAsync().ConfigureAwait(true);
        }
        else
        {
            await SaveEditedItemAsync().ConfigureAwait(true);
        }

        _isAdding = false;
        IsEditing = false;

        async Task SaveAddedItemAsync()
        {
            if (!await _itemProvider.AddItemAsync(CreateItem(EditingItem), cancellationToken).ConfigureAwait(true))
            {
                _logger.LogError("Failed to add the item {ItemName}", EditingItem.Name);
                return;
            }

            _items.Add(EditingItem);
            ItemsSource.Add(EditingItem);
            SelectedItem = EditingItem;
        }

        async Task SaveEditedItemAsync()
        {
            var editedItem = ItemsSource.FirstOrDefault(x => x.Id == SelectedItem?.Id);

            if (editedItem is null)
            {
                IsEditing = false;
                return;
            }

            if (!await _itemProvider.EditItemAsync(CreateItem(EditingItem), cancellationToken).ConfigureAwait(true))
            {
                _logger.LogError("Failed to save the item {ItemName}", EditingItem.Name);
                return;
            }

            editedItem.Name = EditingItem.Name;
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

            RefreshListItem(editedItem);

            OnPropertyChanged(nameof(SelectedItem));
        }
    }

    private async Task<bool> LoadItemsAsync()
    {
        try
        {
            var items = await _itemProvider.GetItemsAsync(CancellationToken.None).ConfigureAwait(true);

            _items.AddEach(items.Select(x => new ItemViewModel(x)));

            ItemTypes = await _itemProvider.GetItemTypesAsync(CancellationToken.None).ConfigureAwait(true);

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

    private void RefreshListItem(ItemViewModel item)
    {
        var index = ItemsSource.IndexOf(item);

        if (index == -1)
        {
            return;
        }

        ItemsSource[index] = item;
    }
}
