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
using RpgFilesGeneratorTools.Models.ItemTypes;
using RpgFilesGeneratorTools.Services;
using RpgFilesGeneratorTools.Toolkit.Async;
using RpgFilesGeneratorTools.Toolkit.Extensions;
using static System.String;

namespace RpgFilesGeneratorTools.ViewModels;

internal class ItemsPageViewModel : ObservableObject
{
    private readonly IItemProvider _itemProvider;
    private readonly ILogger<ItemsPageViewModel> _logger;
    private readonly IList<ItemBase> _items = new List<ItemBase>();

    private ItemBase? _selectedItem;
    private string? _filterText;
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
        CancelCommand = new RelayCommand(CancelEditing);
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

    public ObservableCollection<ItemBase> ItemsSource { get; } = new();

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

    public ItemBase? SelectedItem
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
                // RefreshEditingItem(value);
            }

            DisplayDetails = true;
        }
    }

    public ObservableCollection<string> ItemStatusesSource { get; } = new();

    public ItemViewModel EditingItem { get; } = new("New Item", ItemType.Weapon, ItemSubtype.Axe);

    public bool IsEditing
    {
        get => _isEditing;
        private set => SetProperty(ref _isEditing, value);
    }

    private async Task<bool> LoadItemsAsync()
    {
        try
        {
            var items = await _itemProvider.GetItemsAsync(CancellationToken.None).ConfigureAwait(true);

            _items.AddEach(items.OrderBy(x => x.Type).ThenBy(x => x.Subtype).ThenBy(x => x.Name));

            ItemTypes = await _itemProvider.GetItemTypesAsync(CancellationToken.None).ConfigureAwait(true);

            ItemsSource.AddEach(_items);

            var statuses = new HashSet<string>();

            foreach (var item in _items.Where(x => x is Weapon))
            {
                var weapon = (Weapon)item;

                if (!IsNullOrEmpty(weapon.Status))
                {
                    statuses.Add(weapon.Status);
                }

                if (!IsNullOrEmpty(weapon.Status2))
                {
                    statuses.Add(weapon.Status2);
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

    //private static ItemBase CreateItem(ItemViewModel item)
    //{
    //    var itemToAdd = new Item(item);
    //    return itemToAdd;
    //}

    private void DeleteItemStatus(object? statusIndex)
    {
        if (!int.TryParse(statusIndex?.ToString(), out var index))
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

    private void CancelEditing()
    {
        if (_isAdding)
        {
            DisplayDetails = false;
        }

        _isAdding = false;
        IsEditing = false;
    }

    private void AddNewItem()
    {
        _isAdding = true;
        IsEditing = true;
        DisplayDetails = true;
        RefreshEditingItem(new ItemViewModel("New item", ItemType.Weapon, ItemSubtype.Axe));
    }

    private void RefreshEditingItem(ItemViewModel item)
    {
        CopyItem(source: item, destination: EditingItem);
        OnPropertyChanged(nameof(EditingItem));
    }

    private void ClearSelection()
    {
        DisplayDetails = false;
        SelectedIndex = -1;
    }

    private async Task SaveItemAsync(CancellationToken cancellationToken)
    {
        if (_isAdding && IsNullOrWhiteSpace(EditingItem.Name))
        {
            IsEditing = false;
            return;
        }

        //if (_isAdding)
        //{
        //    await SaveAddedItemAsync().ConfigureAwait(true);
        //}
        //else
        //{
        //    await SaveEditedItemAsync().ConfigureAwait(true);
        //}

        //_isAdding = false;
        //IsEditing = false;

        //async Task SaveAddedItemAsync()
        //{
        //    if (!await _itemProvider.AddItemAsync(CreateItem(EditingItem), cancellationToken).ConfigureAwait(true))
        //    {
        //        _logger.LogError("Failed to add the item {ItemName}", EditingItem.Name);
        //        return;
        //    }

        //    _items.Add(EditingItem);

        //    var index = _items
        //        .OrderBy(x => x.Type)
        //        .ThenBy(x => x.Subtype)
        //        .ThenBy(x => x.Name)
        //        .Select((x, index) => new { x.Id, Index = index })
        //        .FirstOrDefault(x => x.Id == EditingItem.Id)?.Index;

        //    ItemsSource.Insert(index ?? 0, EditingItem);
        //    SelectedItem = EditingItem;
        //}

        //async Task SaveEditedItemAsync()
        //{
        //    var editedItem = ItemsSource.FirstOrDefault(x => x.Id == SelectedItem?.Id);

        //    if (editedItem is null)
        //    {
        //        IsEditing = false;
        //        return;
        //    }

        //    if (!await _itemProvider.EditItemAsync(CreateItem(EditingItem), cancellationToken).ConfigureAwait(true))
        //    {
        //        _logger.LogError("Failed to save the item {ItemName}", EditingItem.Name);
        //        return;
        //    }

        //    CopyItem(source: EditingItem, destination: editedItem);

        //    RefreshListItem(editedItem);

        //    OnPropertyChanged(nameof(SelectedItem));
        //}
    }

    private static void CopyItem(ItemViewModel source, ItemViewModel destination)
    {
        destination.Id = source.Id;
        destination.Name = source.Name;
        destination.Type = source.Type;
        destination.Subtype = source.Subtype;
        destination.Speed = source.Speed;
        destination.RequiredDexterity = source.RequiredDexterity;
        destination.RequiredStrength = source.RequiredStrength;
        destination.RequiredIntelligence = source.RequiredIntelligence;
        destination.MinDamage = source.MinDamage;
        destination.MaxDamage = source.MaxDamage;
        destination.MinBlock = source.MinBlock;
        destination.MaxBlock = source.MaxBlock;
        destination.MinDefense = source.MinDefense;
        destination.MaxDefense = source.MaxDefense;
        destination.Status = source.Status;
        destination.StatusChance = source.StatusChance;
        destination.Status2 = source.Status2;
        destination.Status2Chance = source.Status2Chance;
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

    //private void RefreshListItem(ItemViewModel item)
    //{
    //    var index = ItemsSource.IndexOf(item);

    //    if (index == -1)
    //    {
    //        return;
    //    }

    //    ItemsSource[index] = item;
    //}
}
