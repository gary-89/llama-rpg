using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LlamaRpg.App.Toolkit.Async;
using LlamaRpg.App.Toolkit.Extensions;
using LlamaRpg.App.ViewModels.Monsters;
using LlamaRpg.Models.Monsters;
using LlamaRpg.Services.Randomization;
using LlamaRpg.Services.Readers;
using Microsoft.Extensions.Logging;
using Range = LlamaRpg.Models.Range;

namespace LlamaRpg.App.ViewModels;

internal class MonstersPageViewModel : ObservableObject
{
    private readonly IMonsterProvider _monsterProvider;
    private readonly IRandomizedMonsterProvider _randomizedMonsterProvider;
    private readonly ILogger<MonstersPageViewModel> _logger;

    private readonly AsyncRelayCommand _randomizeCommand;

    public MonstersPageViewModel(IMonsterProvider monsterProvider, IRandomizedMonsterProvider randomizedMonsterProvider, ILogger<MonstersPageViewModel> logger)
    {
        _monsterProvider = monsterProvider;
        _randomizedMonsterProvider = randomizedMonsterProvider;
        _logger = logger;

        ClearCommand = new RelayCommand(() => GeneratedMonsters.Clear());
        _randomizeCommand = new AsyncRelayCommand(RandomizeMonsters, CanRandomizeMonsters);

        TaskInitialize = new NotifyTaskCompletion<bool>(LoadMonstersAsync());
    }

    public ICommand RandomizeCommand => _randomizeCommand;

    public ICommand ClearCommand { get; }

    public NotifyTaskCompletion<bool> TaskInitialize { get; }

    public ObservableCollection<Monster> MonstersSource { get; } = new();

    public ObservableCollection<RandomizedMonster> GeneratedMonsters { get; } = new();

    public MonsterRandomizationSettings Settings { get; } = new();

    private async Task<bool> LoadMonstersAsync()
    {
        try
        {
            var monsters = await _monsterProvider.GetMonsterAsync(CancellationToken.None).ConfigureAwait(true);

            MonstersSource.AddEach(monsters);

            _randomizeCommand.NotifyCanExecuteChanged();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to load items");
        }

        return true;
    }

    private bool CanRandomizeMonsters() => MonstersSource.Count > 0;

    private async Task RandomizeMonsters(CancellationToken cancellationToken)
    {
        try
        {
            var settings = new MonsterRandomizerSettings(
                Settings.AreaType >= 0 ? (MonsterAreaType)Settings.AreaType : null,
                new Range(Settings.MinMonsterLevel, Settings.MaxMonsterLevel));

            await foreach (var monster in _randomizedMonsterProvider.GenerateMonstersAsync(settings, CancellationToken.None).ConfigureAwait(true))
            {
                GeneratedMonsters.Add(monster);
                await Task.Delay(10, cancellationToken).ConfigureAwait(true);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Something went wrong during the monster randomization");
        }
    }
}
