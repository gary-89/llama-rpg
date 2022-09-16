using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

namespace RpgFilesGeneratorTools;

public partial class App
{
    private Window _mainWindow;

    public App()
    {
        Services = ConfigureServices();
        InitializeComponent();
    }

    public static FrameworkElement MainRoot { get; private set; }

    public IServiceProvider Services { get; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var test = Services.GetService<MainViewModel>();

        _mainWindow = new MainWindow(test);
        _mainWindow.Activate();

        MainRoot = _mainWindow.Content as FrameworkElement;
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<TestService>();
        services.AddTransient<MainViewModel>();

        return services.BuildServiceProvider();
    }
}