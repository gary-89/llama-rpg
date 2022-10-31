using System.Threading.Tasks;
using RpgFilesGeneratorTools.Models;

namespace RpgFilesGeneratorTools.Services;

internal sealed class ItemValidator
{
    public Task<bool> ValidateAsync(Item item)
    {
        // TODO
        return Task.FromResult(true);
    }
}
