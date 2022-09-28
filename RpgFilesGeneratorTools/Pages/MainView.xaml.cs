using System;
using RpgFilesGeneratorTools.ViewModels;

namespace RpgFilesGeneratorTools;

internal sealed partial class MainView
{
    public MainView()
    {
        InitializeComponent();
    }

    public MainViewModel ViewModel => DataContext as MainViewModel ?? throw new ArgumentNullException();
}
