using System;
using CryptoTax.ConsoleApp.Commands;
using CryptoTax.ConsoleApp.Commands.Factory;
using CryptoTaxV3.Domain.AppSettings;
using CryptoTaxV3.Domain.Exceptions;
using CryptoTaxV3.Domain.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;

namespace CryptoTax.ConsoleApp.Application
{
    public class App
    {
        private readonly ICommandFactory _commandFactory;
        private readonly IAppSettings _appSettings;
        private readonly ILogger<App> _logger;

        public App(
            ICommandFactory commandFactory,
            IAppSettings appSettings,
            ILogger<App> logger)
        {
            _commandFactory = commandFactory;
            _appSettings = appSettings;
            _logger = logger;
        }

        public void Setup()
        {
            bool setupComplete = false;
            do
            {
                try
                {
                    bool coinsImported = _appSettings.Get<bool>(AppSettingKey.COINS_IMPORTED);
                    if (!coinsImported)
                    {
                        Output.WriteLine("Importing coins...");
                        ExecuteCommand("coins import");
                        SetAppSetting(AppSettingKey.COINS_IMPORTED);
                    }

                    bool coinLookupsImported = _appSettings.Get<bool>(AppSettingKey.COINLOOKUPS_IMPORTED);
                    if (!coinLookupsImported)
                    {
                        Output.Write("Enter path for coin lookups file: ");
                        string path = Console.ReadLine().Trim();
                        ExecuteCommand($"coinlookups import -f {path}");
                        SetAppSetting(AppSettingKey.COINLOOKUPS_IMPORTED);
                    }

                    bool credsImported = _appSettings.Get<bool>(AppSettingKey.CREDS_IMPORTED);
                    if (!credsImported)
                    {
                        Output.Write("Enter path for credentials file: ");
                        string path = Console.ReadLine().Trim();
                        ExecuteCommand($"credentials import -f {path}");
                        SetAppSetting(AppSettingKey.CREDS_IMPORTED);
                    }

                    bool productsImported = _appSettings.Get<bool>(AppSettingKey.PRODUCTS_IMPORTED);
                    if (!productsImported)
                    {
                        Output.WriteLine("Importing products...");
                        ExecuteCommand("products import");
                        SetAppSetting(AppSettingKey.PRODUCTS_IMPORTED);
                    }

                    bool productsActivated = _appSettings.Get<bool>(AppSettingKey.PRODUCTS_ACTIVATED);
                    if (!productsActivated)
                    {
                        Output.Write("Enter path for active products file: ");
                        string path = Console.ReadLine().Trim();
                        ExecuteCommand($"products activate -f {path}");
                        SetAppSetting(AppSettingKey.PRODUCTS_ACTIVATED);
                    }

                    setupComplete = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error setting up application:\n{ex}", ex.ToString());
                    Output.WriteError(ex.Message);
                }

            } while (!setupComplete);

            void SetAppSetting(string key) => _appSettings.AddOrUpdate(key, true);
        }

        public void Run()
        {
            string input;
            do
            {
                Output.WritePrompt();
                input = Console.ReadLine().Trim();
                try
                {
                    ExecuteCommand(input);
                }
                catch (Exception ex)
                {
                    if (ex is ConfigurationException ||
                        ex is InsufficientBalanceException ||
                        ex is ValidationException)
                    {
                        Output.WriteLine(ex.Message);
                    }
                    else
                    {
                        _logger.LogError("Error executing command:\n{ex}", ex.ToString());
                        Output.WriteError(ex.Message);
                    }
                }

            } while (input.ToLower() != "quit");
        }

        private void ExecuteCommand(string input)
        {
            var args = CommandArgs.From(input);
            ICommand command = _commandFactory.GetCommand(args[0]);
            command.Execute(args);
        }
    }
}
