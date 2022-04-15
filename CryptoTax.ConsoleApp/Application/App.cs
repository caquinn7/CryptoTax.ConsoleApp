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
                    ExecuteSetup(AppSettingKey.COINS_IMPORTED, "Importing coins...", "coins import", false);
                    ExecuteSetup(AppSettingKey.COINLOOKUPS_IMPORTED, "Enter path for coin lookups file: ", "coinlookups import -f", true);
                    ExecuteSetup(AppSettingKey.CREDS_IMPORTED, "Enter path for credentials file: ", "credentials import -f", true);
                    ExecuteSetup(AppSettingKey.PRODUCTS_IMPORTED, "Importing products...", "products import", false);
                    ExecuteSetup(AppSettingKey.PRODUCTS_ACTIVATED, "Enter path for active products file: ", "products activate -f", true);
                    setupComplete = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error setting up application:\n{ex}", ex.ToString());
                    Output.WriteError(ex.Message);
                }

            } while (!setupComplete);

            void ExecuteSetup(string appSettingKey, string prompt, string command, bool needInput)
            {
                bool setting = _appSettings.Get<bool>(appSettingKey);
                if (!setting)
                {
                    if (needInput)
                    {
                        Output.Write(ConsoleColor.Blue, prompt);
                        ExecuteCommand($"{command} {Console.ReadLine().Trim()}");
                    }
                    else
                    {
                        Output.WriteLine(ConsoleColor.Blue, prompt);
                        ExecuteCommand(command);
                    }
                    SetAppSetting(appSettingKey);
                }
            }

            void SetAppSetting(string key) => _appSettings.AddOrUpdate(key, true);
        }

        public void Run()
        {
            string input;
            do
            {
                Output.WritePrompt();
                input = Console.ReadLine().Trim();
                if (input.ToLower() == "quit") break;

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
