using System;
using System.Threading;
using System.Threading.Tasks;

namespace LlamaRpg.App.Services;

internal interface IUpdatesDetector
{
    public Task<Version?> GetLastVersionAsync(CancellationToken cancellationToken);
}
