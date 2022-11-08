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
using LlamaRpg.App.Services;
using LlamaRpg.App.Toolkit.Async;
using LlamaRpg.App.ViewModels.Randomizer;
using LlamaRpg.Models.Affixes;
using LlamaRpg.Models.Items;
using LlamaRpg.Models.Items.PrimaryTypes;
using Microsoft.Extensions.Logging;
using LlamaRpg.App.Toolkit.Extensions;

namespace LlamaRpg.App.ViewModels;

internal sealed class RandomizerPageViewModel : ObservableObject
{
    private readonly IItemRandomizerProvider _itemRandomizer;
    private readonly IAffixProvider _affixProvider;
    private readonly IItemProvider _itemProvider;
    private readonly ILogger<RandomizerPageViewModel> _logger;
    private readonly AsyncRelayCommand _exportCommand;
    private readonly RelayCommand _stopCommand;

    private IReadOnlyList<ItemBase> _items = Array.Empty<ItemBase>();
    private IReadOnlyList<Affix> _affixes = Array.Empty<Affix>();
    private string? _fileToExportPath;
    private bool _exportEnabled;
    private bool _canStopRandomization;
    private bool _stopRandomization;

    public RandomizerPageViewModel(IItemRandomizerProvider itemRandomizer, IAffixProvider affixProvider, IItemProvider itemProvider, ILogger<RandomizerPageViewModel> logger)
    {
        _itemRandomizer = itemRandomizer;
        _affixProvider = affixProvider;
        _itemProvider = itemProvider;
        _logger = logger;

        RandomizeCommand = new AsyncRelayCommand(GenerateRandomizedItemsAsync, CanRandomizeItems);
        ClearCommand = new RelayCommand(ClearItemsAndStats);
        _stopCommand = new RelayCommand(StopRandomization, CanStopRandomizationProcess);
        _exportCommand = new AsyncRelayCommand(ExportGeneratedItemsAsync, CanExportItems);

        var openLastGeneratedFileCommand = new RelayCommand(OpenLastGeneratedFile);

        InfoBar = new InfoBar(openLastGeneratedFileCommand);

        TaskCompletion = new NotifyTaskCompletion<int>(InitializeAsync());
    }

    public NotifyTaskCompletion<int> TaskCompletion { get; }

    public AsyncRelayCommand RandomizeCommand { get; }

    public ICommand ClearCommand { get; }

    public ICommand StopCommand => _stopCommand;

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

    public bool CanStopRandomization
    {
        get => _canStopRandomization;
        set
        {
            if (SetProperty(ref _canStopRandomization, value))
            {
                _stopCommand.NotifyCanExecuteChanged();
            }
        }
    }

    private async Task<int> InitializeAsync()
    {
        _affixes = await _affixProvider.GetAffixesAsync(CancellationToken.None).ConfigureAwait(true);
        _items = await _itemProvider.GetItemsAsync(CancellationToken.None).ConfigureAwait(true);

        var itemTypes = await _itemProvider.GetItemTypesAsync(CancellationToken.None).ConfigureAwait(true);
        Settings.ItemTypeWeights.AddEach(itemTypes.Select(x => new ItemTypeWeightDrop(x, 1)));

        RandomizeCommand.NotifyCanExecuteChanged();
        return 0;
    }

    private void ClearItemsAndStats()
    {
        GeneratedItems.Clear();
        Stats.Clear();
        ExportEnabled = false;
    }

    private void StopRandomization()
    {
        _stopRandomization = true;
    }

    private bool CanStopRandomizationProcess()
    {
        return CanStopRandomization;
    }

    private bool CanRandomizeItems()
    {
        return _affixes.Count > 0 && _items.Count > 0;
    }

    private async Task GenerateRandomizedItemsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Generating {NumberOfItems} randomized items...", Settings.NumberOfItemsToGenerate);

            ExportEnabled = false;
            CanStopRandomization = true;

            var numberOfGeneratedItems = 0;
            await foreach (var item in _itemRandomizer.GenerateRandomizedItemsAsync(Settings, cancellationToken).ConfigureAwait(true))
            {
                if (_stopRandomization)
                {
                    _stopRandomization = false;
                    CanStopRandomization = false;
                    _stopCommand.NotifyCanExecuteChanged();
                    ExportEnabled = true;
                    break;
                }

                GeneratedItems.Add(item);

                RefreshStatsOnAddingItem(item);

                numberOfGeneratedItems++;

                if (numberOfGeneratedItems % 100 == 0)
                {
                    await Task.Delay(1, cancellationToken).ConfigureAwait(true);
                }
            }

            stopwatch.Stop();

            _logger.LogInformation("Done in {ElapsedTimeInMillisecond} ms", stopwatch.ElapsedMilliseconds);

            var itemCountPerPowerLevels = GeneratedItems.GroupBy(x => x.PowerLevel).Select(x =>
            {
                var count = x.Count();
                return new ItemCountPerPowerLevel(x.Key, count, (double)count / GeneratedItems.Count);
            });

            Stats.RefreshItemCountPerPowerLevels(itemCountPerPowerLevels);
            Stats.RefreshItemCountPerTypes();

            ExportEnabled = true;
            CanStopRandomization = false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
        }
    }

    private void RefreshStatsOnAddingItem(RandomizedItem item)
    {
        Stats.UpdateOnAddingItem(item);

        if (!ExportEnabled)
        {
            ExportEnabled = true;
        }

        CanStopRandomization = GeneratedItems.Count > 0;
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
