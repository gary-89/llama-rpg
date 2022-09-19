using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using RpgFilesGeneratorTools.Services;
using RpgFilesGeneratorTools.Toolkit.Extensions;
using Serilog;
using System;

namespace RpgFilesGeneratorTools;

public partial class App
{
    private Window? _mainWindow;

    public App()
    {
        Services = ConfigureServices();
        InitializeComponent();
    }

    public static FrameworkElement? MainRoot { get; private set; }

    public static IServiceProvider? Services { get; private set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        if (Services is null)
        {
            throw new InvalidOperationException();
        }

        var mainViewModel = Services.GetRequiredService<MainViewModel>();

        _mainWindow = new MainWindow(mainViewModel);
        _mainWindow.Activate();

        MainRoot = _mainWindow.Content as FrameworkElement;
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IAffixProvider, AffixProvider>();
        services.AddTransient<MainViewModel>();
        services.AddSerilog();

        return services.BuildServiceProvider();
    }
}
