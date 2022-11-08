using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LlamaRpg.App.Toolkit.Extensions;

internal static class ServiceCollectionExtensions
{
    public static void AddSerilog(this ServiceCollection services)
    {
        var flushInterval = new TimeSpan(0, 0, 1);
        var logFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LlamaRpgTools");
        const long FileSizeLimitBytes = (long)1024 * 1024 * 1024 * 10;

        if (!Directory.Exists(logFolderPath))
        {
            Directory.CreateDirectory(logFolderPath);
        }

        var logFilePath = Path.Combine(logFolderPath, "logs.txt");

        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.RollingFile(logFilePath, flushToDiskInterval: flushInterval, fileSizeLimitBytes: FileSizeLimitBytes)
            .CreateLogger();

        services.AddLogging(x => x.AddSerilog(logger: logger, dispose: true));
    }
}
