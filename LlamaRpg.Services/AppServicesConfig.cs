using System.Diagnostics.CodeAnalysis;

namespace LlamaRpg.Services;

[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "The class will be injected.")]
internal sealed class AppServicesConfig
{
    public string AppFolder => AppContext.BaseDirectory;

    public string AssetsFilesFolder => Path.Combine(AppFolder, "Assets", "Datasets");
}
