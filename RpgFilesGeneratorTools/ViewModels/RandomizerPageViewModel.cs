using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using RpgFilesGeneratorTools.Models;
using RpgFilesGeneratorTools.Services;
using RpgFilesGeneratorTools.Toolkit.Async;
using RpgFilesGeneratorTools.ViewModels.Randomizer;

namespace RpgFilesGeneratorTools.ViewModels;

internal sealed class RandomizerPageViewModel : ObservableObject
{
    private readonly IAffixProvider _affixProvider;
    private readonly IItemProvider _itemProvider;
    private readonly ILogger<RandomizerPageViewModel> _logger;
    private readonly AsyncRelayCommand _randomizeCommand;
    private readonly AsyncRelayCommand _exportCommand;
    private readonly Random _random;

    private IReadOnlyList<Item> _items = Array.Empty<Item>();
    private IReadOnlyList<Affix> _affixes = Array.Empty<Affix>();
    private string? _fileToExportPath;
    private bool _exportEnabled;

    public RandomizerPageViewModel(IAffixProvider affixProvider, IItemProvider itemProvider, ILogger<RandomizerPageViewModel> logger)
    {
        _affixProvider = affixProvider;
        _itemProvider = itemProvider;
        _logger = logger;

        _random = new Random();
        _randomizeCommand = new AsyncRelayCommand(GenerateRandomizedItemsAsync, CanRandomizeItems);

        ClearCommand = new RelayCommand(ClearItemsAndStats);
        _exportCommand = new AsyncRelayCommand(ExportGeneratedItems, CanExportItems);
        var openLastGeneratedFileCommand = new RelayCommand(OpenLastGeneratedFile);

        InfoBar = new InfoBar(openLastGeneratedFileCommand);

        TaskCompletion = new NotifyTaskCompletion<int>(InitializeAsync());
    }

    public NotifyTaskCompletion<int> TaskCompletion { get; }

    public ICommand RandomizeCommand => _randomizeCommand;

    public ICommand ClearCommand { get; }

    public ICommand ExportCommand => _exportCommand;

    public ObservableCollection<RandomizedItem> GeneratedItems { get; } = new();

    public RandomizerSettings Settings { get; } = new();

    public RandomizerStats Stats { get; } = new();

    public InfoBar InfoBar { get; }

    public bool ExportEnabled
    {
        get => _exportEnabled;
        set
        {
            if (SetProperty(ref _exportEnabled, value))
            {
                _exportCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public IEnumerable<ItemTypeFrequency> ItemsTypeFrequencies => _items.Select(x => x.Type).Distinct().Select(type => new ItemTypeFrequency(type, 1));

    private async Task<int> InitializeAsync()
    {
        _affixes = await _affixProvider.GetAffixesAsync(CancellationToken.None).ConfigureAwait(true);
        _items = await _itemProvider.GetItemsAsync(CancellationToken.None).ConfigureAwait(true);

        OnPropertyChanged(nameof(ItemsTypeFrequencies));
        _randomizeCommand.NotifyCanExecuteChanged();
        return 0;
    }

    private void ClearItemsAndStats()
    {
        GeneratedItems.Clear();
        Stats.TotalGeneratedItemsCount = 0;
        Stats.RareGeneratedItemsCount = 0;
        Stats.EliteGeneratedItemsCount = 0;
        _exportCommand.NotifyCanExecuteChanged();
    }

    private bool CanRandomizeItems()
    {
        return _affixes.Count > 0 && _items.Count > 0;
    }

    private Task GenerateRandomizedItemsAsync(CancellationToken cancellationToken)
    {
        for (var i = 0; i < Settings.NumberOfItemsToGenerate; i++)
        {
            if (!TryGenerateRandomizedItem(out var item))
            {
                continue;
            }

            GeneratedItems.Add(item);

            RefreshStatsOnAddingItem(item.ItemRarityType);
        }

        return Task.CompletedTask;
    }

    private void RefreshStatsOnAddingItem(ItemRarityType rarity)
    {
        Stats.TotalGeneratedItemsCount++;

        switch (rarity)
        {
            case ItemRarityType.Rare:
                Stats.RareGeneratedItemsCount++;
                break;

            case ItemRarityType.Elite:
                Stats.EliteGeneratedItemsCount++;
                break;
        }

        if (!ExportEnabled)
        {
            ExportEnabled = true;
        }
    }

    private bool TryGenerateRandomizedItem([NotNullWhen(true)] out RandomizedItem? result)
    {
        var index = _random.Next(1, 40);
        var affix = _affixes[index];
        var tier = _random.Next(1, 6);
        var affixInfo = affix.Rules.FirstOrDefault(x => x.Tier == tier);
        var items = _items.Where(x => affixInfo?.ItemType.Contains(x.Type, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
        var item = items.Count > 0 ? items[_random.Next(0, items.Count - 1)] : null;

        if (affixInfo is null || item is null)
        {
            result = default;
            return false;
        }

        var mod = affixInfo.Modifier1Min;

        if (int.TryParse(affixInfo.Modifier1Min, out var modMin) &&
            int.TryParse(affixInfo.Modifier1Min, out var modMax))
        {
            mod = $"{_random.Next(modMin, modMax)}";
        }

        var rarity = GenerateRarity();

        result = new RandomizedItem(
            item.Name,
            $"{affix.Name}: {mod}",
            rarity);

        return true;
    }

    private ItemRarityType GenerateRarity()
    {
        var rarity = _random.Next(0, Settings.EliteItemDropRate * Settings.RareItemDropRate);

        return rarity % Settings.EliteItemDropRate == 0
            ? ItemRarityType.Elite
            : rarity % Settings.RareItemDropRate == 0
                ? ItemRarityType.Rare
                : ItemRarityType.Normal;
    }

    private bool CanExportItems() => ExportEnabled;

    private async Task ExportGeneratedItems(CancellationToken cancellationToken)
    {
        try
        {
            ExportEnabled = false;

            var tempFolderPath = Path.GetTempPath();

            _fileToExportPath = $"{Path.Combine(tempFolderPath, "Export " + DateTime.Now.ToString("yyyyMMdd HHmmss"))}.txt";

            _logger.LogInformation("Exporting items to {Path}.", _fileToExportPath);

            var offset = 0;

            await using (var fileStream = File.Open(_fileToExportPath, FileMode.OpenOrCreate))
            {
                fileStream.Seek(0, SeekOrigin.End);

                foreach (var item in GeneratedItems)
                {
                    var line = $"{item.WeaponType},{item.Affix},{item.ItemRarityType}," + Environment.NewLine;
                    var buffer = System.Text.Encoding.UTF8.GetBytes(line).AsMemory();
                    fileStream.Seek(offset, SeekOrigin.Begin);
                    await fileStream.WriteAsync(buffer, cancellationToken).ConfigureAwait(true);
                    offset += buffer.Length;
                }
            }

            await InfoBar.ShowAsync("File created", $"Items successfully exported to: {_fileToExportPath}", true).ConfigureAwait(true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exporting items failed: {Message}", e.Message);
        }
        finally
        {
            ExportEnabled = true;
        }
    }

    private void OpenLastGeneratedFile()
    {
        if (_fileToExportPath is null)
        {
            return;
        }

        RunSafely(OpenFile);

        Task OpenFile()
        {
            var pi = new ProcessStartInfo(_fileToExportPath)
            {
                UseShellExecute = true,
                FileName = _fileToExportPath,
            };
            Process.Start(pi);
            return Task.CompletedTask;
        }
    }

    private void RunSafely(Func<Task> action, [CallerMemberName] string callerMember = "")
    {
        try
        {
            action.Invoke();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{CallerMember} failed: {Message}", callerMember, e.Message);
        }
    }
}

internal sealed class ItemTypeFrequency
{
    public ItemTypeFrequency(string name, int frequency)
    {
        Name = name;
        Frequency = frequency;
    }

    public string Name { get; set; }
    public int Frequency { get; set; }
}
