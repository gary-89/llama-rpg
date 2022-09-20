using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using RpgFilesGeneratorTools.Models;
using RpgFilesGeneratorTools.Services;
using RpgFilesGeneratorTools.Toolkit.Async;
using RpgFilesGeneratorTools.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RpgFilesGeneratorTools.ViewModels;

internal class ItemsPageViewModel : ObservableObject
{
    private readonly IItemProvider _itemProvider;
    private readonly ILogger<ItemsPageViewModel> _logger;

    private Item? _selectedItem;
    private string? _filterText;
    private IReadOnlyList<Item> _items = new List<Item>();

    public ItemsPageViewModel(IItemProvider itemProvider, ILogger<ItemsPageViewModel> logger)
    {
        _itemProvider = itemProvider;
        _logger = logger;

        TaskInitialize = new NotifyTaskCompletion<bool>(LoadItemsAsync());
    }

    public NotifyTaskCompletion<bool> TaskInitialize { get; }

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

    public Item? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
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

        ItemsSource.AddEach(string.IsNullOrWhiteSpace(FilterText)
            ? _items
            : _items.Where(x => x.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                                x.Type.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                                x.SubType.Contains(FilterText, StringComparison.OrdinalIgnoreCase)));
    }
}
