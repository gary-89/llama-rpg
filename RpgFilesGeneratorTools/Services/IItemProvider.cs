using RpgFilesGeneratorTools.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RpgFilesGeneratorTools.Services;

internal interface IItemProvider
{
    Task<IReadOnlyList<Item>> GetItemsAsync(CancellationToken cancellationToken);
}
