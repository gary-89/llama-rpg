using CommunityToolkit.Mvvm.ComponentModel;

namespace LlamaRpg.App.ViewModels.Monsters;

internal sealed class MonsterRandomizationSettings : ObservableObject
{
    private int _areaType;

    public int AreaType
    {
        get => _areaType;
        set => SetProperty(ref _areaType, value);
    }
}
