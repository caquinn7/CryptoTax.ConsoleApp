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
                    ExecuteSetup(AppSettingKey.COINS_IMPORTED, "Importing coins...", $"coins import");
                    ExecuteSetupWithInput(AppSettingKey.COINLOOKUPS_IMPORTED, "Enter path for coin lookups file: ", $"coinlookups import -f");
                    ExecuteSetupWithInput(AppSettingKey.CREDS_IMPORTED, "Enter path for credentials file: ", $"credentials import -f");
                    ExecuteSetup(AppSettingKey.PRODUCTS_IMPORTED, "Importing products...", $"products import");
                    ExecuteSetupWithInput(AppSettingKey.PRODUCTS_ACTIVATED, "Enter path for active products file: ", $"products activate -f");
                    setupComplete = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error setting up application:\n{ex}", ex.ToString());
                    Output.WriteError(ex.Message);
                }

            } while (!setupComplete);

            void ExecuteSetupWithInput(string appSettingKey, string prompt, string command)
            {
                bool setting = _appSettings.Get<bool>(appSettingKey);
                if (!setting)
                {
                    Output.Write(ConsoleColor.Blue, prompt);
                    ExecuteCommand($"{command} {Console.ReadLine().Trim()}");
                    SetAppSetting(appSettingKey);
                }
            }

            void ExecuteSetup(string appSettingKey, string prompt, string command)
            {
                bool setting = _appSettings.Get<bool>(appSettingKey);
                if (!setting)
                {
                    Output.WriteLine(ConsoleColor.Blue, prompt);
                    ExecuteCommand(command);
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
