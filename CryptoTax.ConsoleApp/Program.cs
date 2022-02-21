using System;
using System.IO;
using CryptoTax.ConsoleApp.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CryptoTax.ConsoleApp
{
    class Program
    {
        private static readonly IConfiguration _config;

        static Program()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT")}.json", optional: true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables()
                .Build();
        }

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_config)
                .CreateLogger();

            Log.Logger.Debug("Application starting...");
            try
            {
                IHost host = Host
                    .CreateDefaultBuilder()
                    .RegisterDependencies(_config)
                    .UseSerilog()
                    .SetupDatabase(_config)
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
                Log.Logger.Debug("Application stopping...");
                Log.CloseAndFlush();
            }
        }
    }
}
