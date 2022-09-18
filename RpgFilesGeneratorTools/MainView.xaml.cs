using Microsoft.UI.Xaml.Controls;
using System;

namespace RpgFilesGeneratorTools;

internal sealed partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    public MainViewModel ViewModel => DataContext as MainViewModel ?? throw new ArgumentNullException();
}