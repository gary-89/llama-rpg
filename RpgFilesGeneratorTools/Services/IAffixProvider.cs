using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LlamaRpg.Models.Affixes;

namespace RpgFilesGeneratorTools.Services;

internal interface IAffixProvider
{
    ValueTask<IReadOnlyList<Affix>> GetAffixesAsync(CancellationToken cancellationToken);
}
