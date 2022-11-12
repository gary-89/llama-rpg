using LlamaRpg.Models.Items.PrimaryTypes;

namespace LlamaRpg.Services.Validators;

internal sealed class ItemValidator
{
    public Task<bool> ValidateAsync(ItemBase item)
    {
        // TODO
        return Task.FromResult(true);
    }
}
