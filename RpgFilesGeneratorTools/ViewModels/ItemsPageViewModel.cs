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
    private int _selectedIndex;

    public ItemsPageViewModel(IItemProvider itemProvider, ILogger<ItemsPageViewModel> logger)
    {
        _itemProvider = itemProvider;
        _logger = logger;

        TaskInitialize = new NotifyTaskCompletion<bool>(LoadItemsAsync());

        ResetSelectionCommand = new RelayCommand(() =>
        {
            SelectedIndex = -1;
        });

        EditCommand = new RelayCommand(() => IsEditing = true);
        SaveCommand = new RelayCommand(() => IsEditing = false);
        CancelCommand = new RelayCommand(() => IsEditing = false);
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
        set => SetProperty(ref _displayDetails, value);
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
            if (value is not null && SetProperty(ref _selectedItem, value))
            {
                EditingItem = value;
                DisplayDetails = true;
            }
        }
    }

    public Item? EditingItem
    {
        get => _editingItem;
        set => SetProperty(ref _editingItem, value);
    }

    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    private async Task<bool> LoadItemsAsync()
    {
        try
        {
            _items = await _itemProvider.GetItemsAsync(CancellationToken.None);

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
                                    x.SubType.Contains(FilterText, StringComparison.OrdinalIgnoreCase)));
    }
}
