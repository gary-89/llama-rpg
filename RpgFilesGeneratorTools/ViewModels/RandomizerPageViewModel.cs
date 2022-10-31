using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
using RpgFilesGeneratorTools.ViewModels.Randomizer;

namespace RpgFilesGeneratorTools.ViewModels;

internal sealed class RandomizerPageViewModel : ObservableObject
{
    private readonly IItemRandomizerProvider _itemRandomizer;
    private readonly IAffixProvider _affixProvider;
    private readonly IItemProvider _itemProvider;
    private readonly ILogger<RandomizerPageViewModel> _logger;
    private readonly AsyncRelayCommand _randomizeCommand;
    private readonly AsyncRelayCommand _exportCommand;

    private IReadOnlyList<Item> _items = Array.Empty<Item>();
    private IReadOnlyList<Affix> _affixes = Array.Empty<Affix>();
    private string? _fileToExportPath;
    private bool _exportEnabled;

    public RandomizerPageViewModel(IItemRandomizerProvider itemRandomizer, IAffixProvider affixProvider, IItemProvider itemProvider, ILogger<RandomizerPageViewModel> logger)
    {
        _itemRandomizer = itemRandomizer;
        _affixProvider = affixProvider;
        _itemProvider = itemProvider;
        _logger = logger;

        _randomizeCommand = new AsyncRelayCommand(GenerateRandomizedItemsAsync, CanRandomizeItems);

        ClearCommand = new RelayCommand(ClearItemsAndStats);
        _exportCommand = new AsyncRelayCommand(ExportGeneratedItemsAsync, CanExportItems);
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

    private async Task<int> InitializeAsync()
    {
        _affixes = await _affixProvider.GetAffixesAsync(CancellationToken.None).ConfigureAwait(true);
        _items = await _itemProvider.GetItemsAsync(CancellationToken.None).ConfigureAwait(true);

        var itemTypes = await _itemProvider.GetItemTypesAsync(CancellationToken.None).ConfigureAwait(true);
        Settings.ItemTypeWeights.AddEach(itemTypes.Select(x => new ItemTypeWeightDrop(x, 1)));

        _randomizeCommand.NotifyCanExecuteChanged();
        return 0;
    }

    private void ClearItemsAndStats()
    {
        GeneratedItems.Clear();
        Stats.Clear();
        _exportCommand.NotifyCanExecuteChanged();
    }

    private bool CanRandomizeItems()
    {
        return _affixes.Count > 0 && _items.Count > 0;
    }

    private async Task GenerateRandomizedItemsAsync(CancellationToken cancellationToken)
    {
        await RunSafely(InternalGenerateRandomizedItemsAsync).ConfigureAwait(true);

        async Task InternalGenerateRandomizedItemsAsync()
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Generating {NumberOfItems} randomized items...", Settings.NumberOfItemsToGenerate);

            await foreach (var item in _itemRandomizer.GenerateRandomizedItemsAsync(Settings, cancellationToken).ConfigureAwait(true))
            {
                GeneratedItems.Add(item);
                RefreshStatsOnAddingItem(item);
            }

            Stats.RefreshItemCountPerTypes();

            stopwatch.Stop();

            _logger.LogInformation("Done in {ElapsedTimeInMillisecond} ms", stopwatch.ElapsedMilliseconds);
        }
    }

    private void RefreshStatsOnAddingItem(RandomizedItem item)
    {
        Stats.UpdateOnAddingItem(item);

        if (!ExportEnabled)
        {
            ExportEnabled = true;
        }
    }

    private bool CanExportItems() => ExportEnabled;

    private async Task ExportGeneratedItemsAsync(CancellationToken cancellationToken)
    {
        try
        {
            ExportEnabled = false;

            var tempFolderPath = Path.GetTempPath();

            _fileToExportPath = $"{Path.Combine(tempFolderPath, "Export " + DateTime.Now.ToString("yyyyMMdd HHmmss"))}.txt";

            _logger.LogInformation("Exporting items to {Path}.", _fileToExportPath);

            var lines = GeneratedItems.Select(x => string.Join(',', x.ItemType, x.Affixes, x.ItemRarityType.ToString()));

            await File.WriteAllLinesAsync(_fileToExportPath, lines, Encoding.UTF8, cancellationToken).ConfigureAwait(true);

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

        RunSafely(OpenFile).GetAwaiter().GetResult();

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

    private async Task RunSafely(Func<Task> action, [CallerMemberName] string callerMember = "")
    {
        try
        {
            await action.Invoke().ConfigureAwait(true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{CallerMember} failed: {Message}", callerMember, e.Message);
        }
    }
}
