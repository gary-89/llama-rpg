namespace LlamaRpg.Services;

internal sealed class AppConfig
{
    public string AppFolder => AppContext.BaseDirectory;

    public string AssetsFilesFolder => Path.Combine(AppFolder, "Assets", "Datasets");
}
