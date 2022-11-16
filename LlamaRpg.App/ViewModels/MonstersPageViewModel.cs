using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LlamaRpg.App.Toolkit.Async;
using LlamaRpg.App.Toolkit.Extensions;
using LlamaRpg.App.ViewModels.Monsters;
using LlamaRpg.Models.Items;
using LlamaRpg.Models.Monsters;
using LlamaRpg.Services.Readers;
using Microsoft.Extensions.Logging;

namespace LlamaRpg.App.ViewModels;

internal class MonstersPageViewModel : ObservableObject
{
    private readonly IMonsterProvider _monsterProvider;
    private readonly ILogger<MonstersPageViewModel> _logger;

    private readonly AsyncRelayCommand _randomizeCommand;

    public MonstersPageViewModel(IMonsterProvider monsterProvider, ILogger<MonstersPageViewModel> logger)
    {
        _monsterProvider = monsterProvider;
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
            var random = new Random();

            var source = MonstersSource.Where(x => x.Area == Settings.AreaType).ToList();

            for (var i = 0; i < 100; i++)
            {
                var index = random.Next(0, source.Count);
                var monster = source[index];

                var uniqueBoss = random.Next(0, 100);

                var uniqueMonsterType = uniqueBoss > 30
                    ? UniqueMonsterType.Normal
                    : uniqueBoss > 10
                        ? UniqueMonsterType.Boss
                        : UniqueMonsterType.SuperBoss;

                var generatedMonster = new RandomizedMonster(
                    i + 1,
                    monster.Name,
                    monster.Type,
                    uniqueMonsterType,
                    monster.Area,
                    monster.Level + (uniqueMonsterType switch { UniqueMonsterType.Boss => 5, UniqueMonsterType.SuperBoss => 10, _ => 0 }),
                    Array.Empty<Item>());

                GeneratedMonsters.Add(generatedMonster);

                await Task.Delay(10, cancellationToken).ConfigureAwait(true);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Something went wrong during the monster randomization");
        }
    }
}
