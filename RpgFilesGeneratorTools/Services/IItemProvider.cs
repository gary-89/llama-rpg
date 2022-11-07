using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LlamaRpg.Models.Items;
using LlamaRpg.Models.Models.ItemTypes;

namespace RpgFilesGeneratorTools.Services;

internal interface IItemProvider
{
    Task<bool> AddItemAsync(ItemBase item, CancellationToken cancellationToken);

    Task<bool> EditItemAsync(ItemBase item, CancellationToken cancellationToken);

    Task<bool> DeleteItemAsync(ItemBase item, CancellationToken cancellationToken);

    ValueTask<IReadOnlyList<ItemBase>> GetItemsAsync(CancellationToken cancellationToken);

    ValueTask<IReadOnlyList<ItemType>> GetItemTypesAsync(CancellationToken cancellationToken);
}
