using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using RpgFilesGeneratorTools.Services;
using RpgFilesGeneratorTools.Toolkit.Extensions;
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;

namespace RpgFilesGeneratorTools;

public partial class App
{
    private Window? _mainWindow;
    private ILogger<App>? _logger;

    public App()
    {
        UnhandledException += OnUnhandledException;

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

        _logger = Services.GetRequiredService<ILogger<App>>();

        _logger.LogInformation("App launched");

        var mainViewModel = Services.GetRequiredService<MainViewModel>();

        _mainWindow = new MainWindow(mainViewModel)
        {
            Title = "Llama RPG editor",
        };

        _mainWindow.Activate();

        MainRoot = _mainWindow.Content as FrameworkElement;
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<AppConfig>();
        services.AddSingleton<IAffixProvider, AffixProvider>();
        services.AddSingleton<IItemProvider, ItemProvider>();
        services.AddSingleton<IItemRandomizerProvider, ItemRandomizerProvider>();
        services.AddTransient<MainViewModel>();
        services.AddSerilog();

        return services.BuildServiceProvider();
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        _logger?.LogCritical(e.Exception, "{Type} exception: {Message}", (e.Handled ? "Handled" : "Unhandled"), e.Message);
    }
}
