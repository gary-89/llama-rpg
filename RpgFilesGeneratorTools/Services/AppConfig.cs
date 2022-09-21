using System;
using System.IO;

namespace RpgFilesGeneratorTools.Services;

internal sealed class AppConfig
{
    public string AppFolder => AppContext.BaseDirectory;

    public string AssetsFilesFolder => Path.Combine(AppFolder, "Assets", "Files");
}
