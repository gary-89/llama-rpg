using System.Collections.Generic;
using System.Threading;
using LlamaRpg.Models.Items;
using RpgFilesGeneratorTools.ViewModels.Randomizer;

namespace RpgFilesGeneratorTools.Services;

internal interface IItemRandomizerProvider
{
    public IAsyncEnumerable<RandomizedItem> GenerateRandomizedItemsAsync(RandomizerSettings settings, CancellationToken cancellationToken);
}
