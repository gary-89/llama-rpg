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

    public ItemsPageViewModel(IItemProvider itemProvider, ILogger<ItemsPageViewModel> logger)
    {
        _itemProvider = itemProvider;
        _logger = logger;

        ResetSelectionCommand = new RelayCommand(() => DisplayDetails = false);
        EditCommand = new RelayCommand(() => IsEditing = true);
        CancelCommand = new RelayCommand(() => IsEditing = false);
        SaveCommand = new RelayCommand(SaveItem);

        TaskInitialize = new NotifyTaskCompletion<bool>(LoadItemsAsync());
    }

    public NotifyTaskCompletion<bool> TaskInitialize { get; }

    public ICommand ResetSelectionCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

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

    public bool DisplayDetails
    {
        get => _displayDetails;
        private set => SetProperty(ref _displayDetails, value);
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

        editedItem.Speed = EditingItem.Speed;
        editedItem.RequiredDexterity = EditingItem.RequiredDexterity;
        editedItem.RequiredStrength = EditingItem.RequiredStrength;

        OnPropertyChanged(nameof(SelectedItem));

        IsEditing = false;
    }

    private async Task<bool> LoadItemsAsync()
    {
        try
        {
            _items = await _itemProvider.GetItemsAsync(CancellationToken.None).ConfigureAwait(true);

            ItemsSource.AddEach(_items);
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
            string.IsNullOrWhiteSpace(FilterText)
                ? _items
                : _items.Where(x => x.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                                    x.Type.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                                    x.Subtype.Contains(FilterText, StringComparison.OrdinalIgnoreCase)));
    }
}
