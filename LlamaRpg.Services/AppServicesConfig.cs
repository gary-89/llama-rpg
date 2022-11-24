using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace LlamaRpg.Services;

[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "The class will be injected.")]
internal sealed class AppServicesConfig
{
    public string AppFolder => Directory.GetParent(Assembly.GetExecutingAssembly().Location)?.FullName
                               ?? throw new InvalidOperationException("Impossible to find assembly location");

    public string AssetsFilesFolder => Path.Combine(AppFolder, "Assets", "Datasets");
}
