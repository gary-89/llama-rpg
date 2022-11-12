using LlamaRpg.Models.Items;
using LlamaRpg.Models.Items.PrimaryTypes;

namespace LlamaRpg.Services.Readers;

public interface IItemProvider
{
    Task<bool> AddItemAsync(ItemBase item, CancellationToken cancellationToken);

    Task<bool> EditItemAsync(ItemBase item, CancellationToken cancellationToken);

    Task<bool> DeleteItemAsync(ItemBase item, CancellationToken cancellationToken);

    ValueTask<IReadOnlyList<ItemBase>> GetItemsAsync(CancellationToken cancellationToken);

    ValueTask<IReadOnlyList<ItemType>> GetItemTypesAsync(CancellationToken cancellationToken);
}
