using LlamaRpg.Models.Items;
using LlamaRpg.Models.Randomizer;

namespace LlamaRpg.Services.Randomization;

public interface IItemRandomizerProvider
{
    public IAsyncEnumerable<RandomizedItem> GenerateItemsAsync(RandomizerSettings settings, CancellationToken cancellationToken);
}
