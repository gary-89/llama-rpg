using LlamaRpg.Models.Items;
using LlamaRpg.Models.Items.PrimaryTypes;

namespace LlamaRpg.Services.Readers;

public interface IItemProvider
{
    ValueTask<IReadOnlyList<ItemBase>> GetItemsAsync(CancellationToken cancellationToken);

    ValueTask<IReadOnlyList<ItemType>> GetItemTypesAsync(CancellationToken cancellationToken);
}
