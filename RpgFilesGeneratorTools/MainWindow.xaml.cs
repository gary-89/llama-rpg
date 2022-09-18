namespace RpgFilesGeneratorTools;

internal sealed partial class MainWindow
{
    public MainWindow(MainViewModel mainViewModel)
    {
        MainViewModel = mainViewModel;
        InitializeComponent();
    }

    public MainViewModel MainViewModel { get; }
}