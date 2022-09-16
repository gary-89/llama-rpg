// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

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