using CommunityToolkit.Mvvm.ComponentModel;
using LlamaRpg.Models.Monsters;

namespace LlamaRpg.App.ViewModels.Monsters;

internal sealed class MonsterRandomizationSettings : ObservableObject
{
    private int _monsterAreaType;

    public int MinMonsterLevel { get; set; } = 1;

    public int MaxMonsterLevel { get; set; } = 99;

    public int AreaType
    {
        get => _monsterAreaType;
        set
        {
            if (SetProperty(ref _monsterAreaType, value))
            {
                OnPropertyChanged(nameof(MonsterAreaTypeDisplayName));
            }
        }
    }

    public string MonsterAreaTypeDisplayName
    {
        get
        {
            return AreaType switch
            {
                -1 => "Any",
                0 => "Normal",
                (int)MonsterAreaType.Fire => "Fire",
                (int)MonsterAreaType.Cold => "Cold",
                (int)MonsterAreaType.Poison => "Poison",
                (int)MonsterAreaType.Electric => "Electric",
                _ => string.Empty
            };
        }
    }
}
