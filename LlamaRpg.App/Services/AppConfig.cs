using System;
using System.IO;

namespace LlamaRpg.App.Services;

internal sealed class AppConfig
{
    public string AppFolder => AppContext.BaseDirectory;

    public string AssetsFilesFolder => Path.Combine(AppFolder, "Assets", "Datasets");
}
