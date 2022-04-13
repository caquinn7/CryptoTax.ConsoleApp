using System;
using System.IO;
using CryptoTax.ConsoleApp.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace CryptoTax.ConsoleApp
{
    class Program
    {
        static IConfiguration Configuration => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables()
                .Build();

        static void Main(string[] args)
        {
            Log.Logger = CreateLogger();
            Log.Logger.Information("Application starting...");
            try
            {
                IHost host = Host
                    .CreateDefaultBuilder()
                    .RegisterDependencies(Configuration)
                    .UseSerilog()
                    .SetupDatabase(Configuration)
                    .Build();

                var app = ActivatorUtilities.CreateInstance<App>(host.Services);
                app.Setup();
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.Logger.Information("Application stopping...");
                Log.CloseAndFlush();
            }
        }

        static ILogger CreateLogger()
        {
            string fileTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Level:u3} {Message:lj}{NewLine}{Exception}";
            string consoleTemplate = "[{Timestamp:HH:mm:ss}] {Message:lj}{NewLine}";

            return new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Logger(config =>
                {
                    config
                        .Filter.ByIncludingOnly(logEvent => logEvent.Properties.ContainsKey("Http"))
                        .WriteTo.File("logs/httplog.txt", outputTemplate: fileTemplate);
                })
                .WriteTo.Logger(config =>
                {
                    config
                        .Filter.ByIncludingOnly(logEvent => logEvent.Properties.ContainsKey("Console"))
                        .WriteTo.Console(outputTemplate: consoleTemplate, theme: AnsiConsoleTheme.Code);
                })
                .WriteTo.Logger(config =>
                {
                    config
                        .Filter.ByIncludingOnly(logEv => !logEv.Properties.ContainsKey("Console") && !logEv.Properties.ContainsKey("Http"))
                        .WriteTo.File("logs/log.txt", outputTemplate: fileTemplate);
                })
                .CreateLogger();
        }
    }
}
