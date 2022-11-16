using LlamaRpg.Models.Monsters;

namespace LlamaRpg.Services.Randomization;

public interface IRandomizedMonsterProvider
{
    public IAsyncEnumerable<RandomizedMonster> GenerateMonstersAsync(MonsterRandomizerSettings settings, CancellationToken cancellationToken);
}
