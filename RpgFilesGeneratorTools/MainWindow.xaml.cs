namespace RpgFilesGeneratorTools;

internal sealed partial class MainWindow
{
    public MainWindow(MainViewModel mainViewModel)
    {
        MainViewModel = mainViewModel;
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
    }

    public MainViewModel MainViewModel { get; }
}
