using System.Threading.Tasks;
using RpgFilesGeneratorTools.Models.ItemTypes;

namespace RpgFilesGeneratorTools.Services;

internal sealed class ItemValidator
{
    public Task<bool> ValidateAsync(ItemBase item)
    {
        // TODO
        return Task.FromResult(true);
    }
}
