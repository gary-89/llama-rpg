using System;
using LlamaRpg.App.ViewModels;

namespace LlamaRpg.App.Views;

internal sealed partial class MainView
{
    public MainView()
    {
        InitializeComponent();
    }

    public MainViewModel ViewModel => DataContext as MainViewModel ?? throw new ArgumentNullException();
}
