using System.Collections.Generic;
using System.Threading;
using RpgFilesGeneratorTools.Models;
using RpgFilesGeneratorTools.ViewModels.Randomizer;

namespace RpgFilesGeneratorTools.Services;

internal interface IItemRandomizerProvider
{
    public IAsyncEnumerable<RandomizedItem> GenerateRandomizedItemsAsync(RandomizerSettings settings, CancellationToken cancellationToken);
}
