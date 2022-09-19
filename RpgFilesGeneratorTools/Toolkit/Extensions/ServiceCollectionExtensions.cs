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
        var logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LlamaRpg", "app.log");

        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.RollingFile(@"C:\Users\matte\Desktop\logs.txt", flushToDiskInterval: flushInterval)
            //.WriteTo.RollingFile(logFilePath, flushToDiskInterval: flushInterval, shared: true, fileSizeLimitBytes: (long)1024 * 1024 * 1024 * 10)
            //.WriteTo.RollingFile(@"C:\Users\matte\Desktop\app.log", flushToDiskInterval: flushInterval, shared: true, fileSizeLimitBytes: (long)1024 * 1024 * 1024 * 10)
            //.WriteTo.RollingFile(@"C:\Users\matte\AppData\Local\LlamaRpg\app.log", flushToDiskInterval: flushInterval, shared: true, fileSizeLimitBytes: (long)1024 * 1024 * 1024 * 10)
            .CreateLogger();

        services.AddLogging(x => x.AddSerilog(logger: logger, dispose: true));
    }
}
