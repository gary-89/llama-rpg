using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RpgFilesGeneratorTools.Models;

namespace RpgFilesGeneratorTools.Services;

internal interface IItemProvider
{
    ValueTask<IReadOnlyList<Item>> GetItemsAsync(CancellationToken cancellationToken);

    ValueTask<IReadOnlyList<ItemType>> GetItemTypesAsync(CancellationToken cancellationToken);
}
