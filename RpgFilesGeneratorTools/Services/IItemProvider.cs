using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RpgFilesGeneratorTools.Models;

namespace RpgFilesGeneratorTools.Services;

internal interface IItemProvider
{
    Task<bool> AddItemAsync(Item item, CancellationToken cancellationToken);

    Task<bool> EditItemAsync(Item item, CancellationToken cancellationToken);

    Task<bool> DeleteItemAsync(Item item, CancellationToken cancellationToken);

    ValueTask<IReadOnlyList<Item>> GetItemsAsync(CancellationToken cancellationToken);

    ValueTask<IReadOnlyList<ItemType>> GetItemTypesAsync(CancellationToken cancellationToken);
}
