using System;

namespace LlamaRpg.App.Services;

internal sealed class AppConfig
{
    public Version CurrentVersion { get; } = new(0, 0, 7, 0);
}
