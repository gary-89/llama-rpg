using System.Threading.Tasks;
using LlamaRpg.Models.Models.ItemTypes;

namespace RpgFilesGeneratorTools.Services;

internal sealed class ItemValidator
{
    public Task<bool> ValidateAsync(ItemBase item)
    {
        // TODO
        return Task.FromResult(true);
    }
}
