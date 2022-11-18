namespace LlamaRpg.App.Dialogs;

internal sealed partial class AboutView
{
    public AboutView(Version currentVersion)
    {
        InitializeComponent();

        CurrentVersion = currentVersion;
        Year = DateTime.Now.Year;
    }

    public Version CurrentVersion { get; }

    public int Year { get; }
}
