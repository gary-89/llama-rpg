namespace LlamaRpg.Services;

internal sealed class AppServicesConfig
{
    public string AppFolder => AppContext.BaseDirectory;

    public string AssetsFilesFolder => Path.Combine(AppFolder, "Assets", "Datasets");
}
