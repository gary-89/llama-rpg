using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

namespace RpgFilesGeneratorTools;

public partial class App
{
    private Window? _mainWindow;

    public App()
    {
        InitializeComponent();
    }

    public static FrameworkElement? MainRoot { get; private set; }

    public static IServiceProvider Services => ConfigureServices();

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var mainViewModel = Services.GetRequiredService<MainViewModel>();

        _mainWindow = new MainWindow(mainViewModel);
        _mainWindow.Activate();

        MainRoot = _mainWindow.Content as FrameworkElement;
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<RandomNumberProvider>();
        services.AddTransient<MainViewModel>();

        return services.BuildServiceProvider();
    }
}