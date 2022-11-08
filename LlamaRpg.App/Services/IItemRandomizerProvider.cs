using System.Collections.Generic;
using System.Threading;
using LlamaRpg.App.ViewModels.Randomizer;
using LlamaRpg.Models.Items;

namespace LlamaRpg.App.Services;

internal interface IItemRandomizerProvider
{
    public IAsyncEnumerable<RandomizedItem> GenerateRandomizedItemsAsync(RandomizerSettings settings, CancellationToken cancellationToken);
}
