using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;

namespace RpgFilesGeneratorTools.Toolkit.Extensions;

internal static class ServiceCollectionExtensions
{
    public static void AddSerilog(this ServiceCollection services)
    {
        var flushInterval = new TimeSpan(0, 0, 1);
        var logFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LlamaRpgTools");
        const long fileSizeLimitBytes = (long)1024 * 1024 * 1024 * 10;

        if (!Directory.Exists(logFolderPath))
        {
            Directory.CreateDirectory(logFolderPath);
        }

        var logFilePath = Path.Combine(logFolderPath, "app.log");

#if DEBUG
        logFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "app.log");
#endif

        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.RollingFile(logFilePath, flushToDiskInterval: flushInterval, fileSizeLimitBytes: fileSizeLimitBytes)
            .CreateLogger();

        services.AddLogging(x => x.AddSerilog(logger: logger, dispose: true));
    }
}
