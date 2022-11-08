using System.Threading.Tasks;
using LlamaRpg.Models.Items.PrimaryTypes;

namespace LlamaRpg.App.Services;

internal sealed class ItemValidator
{
    public Task<bool> ValidateAsync(ItemBase item)
    {
        // TODO
        return Task.FromResult(true);
    }
}
