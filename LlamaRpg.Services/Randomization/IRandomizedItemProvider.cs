using LlamaRpg.Models.Items;
using LlamaRpg.Models.Randomizer;

namespace LlamaRpg.Services.Randomization;

public interface IRandomizedItemProvider
{
    public IAsyncEnumerable<RandomizedItem> GenerateItemsAsync(ItemRandomizerSettings settings, CancellationToken cancellationToken);
}
