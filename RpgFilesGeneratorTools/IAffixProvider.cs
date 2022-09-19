using RpgFilesGeneratorTools.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RpgFilesGeneratorTools;

internal interface IAffixProvider
{
    ValueTask<IReadOnlyList<Affix>> GetAffixesAsync(CancellationToken cancellationToken);
}
