using LlamaRpg.Models.Monsters;

namespace LlamaRpg.Services.Readers;

public interface IMonsterProvider
{
    ValueTask<IReadOnlyList<Monster>> GetMonsterAsync(CancellationToken cancellationToken);
}
