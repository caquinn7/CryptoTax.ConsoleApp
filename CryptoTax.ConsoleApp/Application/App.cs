﻿using System;
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
            bool coinsImported = _appSettings.Get<bool>(AppSettingKey.COINS_IMPORTED);
            if (!coinsImported)
            {
                Output.WriteLine("Importing coins...");
                ExecuteCommand("coins import");
                SetAppSetting(AppSettingKey.COINS_IMPORTED);
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
                ExecuteCommand("products");
                SetAppSetting(AppSettingKey.PRODUCTS_IMPORTED);
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
                ExecuteCommand(input);

            } while (input.ToLower() != "quit");
        }

        private void ExecuteCommand(string input)
        {
            try
            {
                var args = CommandArgs.From(input);
                ICommand command = _commandFactory.GetCommand(args[0]);
                command.Execute(args);
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
        }
    }
}
