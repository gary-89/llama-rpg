using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RpgFilesGeneratorTools
{
    internal sealed partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        public MainViewModel ViewModel => DataContext as MainViewModel ?? throw new ArgumentNullException();
    }
}
