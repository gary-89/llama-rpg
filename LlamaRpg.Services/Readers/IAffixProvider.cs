using LlamaRpg.Models.Affixes;

namespace LlamaRpg.Services.Readers;

public interface IAffixProvider
{
    ValueTask<IReadOnlyList<Affix>> GetAffixesAsync(CancellationToken cancellationToken);
}
