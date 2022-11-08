using System.Threading.Tasks;
using LlamaRpg.Models.Items.PrimaryTypes;

namespace RpgFilesGeneratorTools.Services;

internal sealed class ItemValidator
{
    public Task<bool> ValidateAsync(ItemBase item)
    {
        // TODO
        return Task.FromResult(true);
    }
}
