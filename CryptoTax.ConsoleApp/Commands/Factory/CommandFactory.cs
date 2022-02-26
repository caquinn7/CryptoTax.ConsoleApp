using System;
using CryptoTax.ConsoleApp.Commands.Implementations;
using CryptoTaxV3.Domain.AppSettings;
using CryptoTaxV3.Domain.CoinLookups;
using CryptoTaxV3.Domain.Coins;
using CryptoTaxV3.Domain.Credentials;
using CryptoTaxV3.Domain.FormEntries;
using CryptoTaxV3.Domain.Products;
using CryptoTaxV3.Domain.Sources;
using CryptoTaxV3.Domain.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoTax.ConsoleApp.Commands.Factory
{
    public class CommandFactory : ICommandFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICommand GetCommand(string arg) => arg.ToLower() switch
        {
            "accounts" => GetAccountCommand(),
            "settings" => GetAppSettingsCommand(),
            "coins" => GetCoinCommand(),
            "coinlookups" => GetCoinLookupCommand(),
            "credentials" => GetCredentialCommand(),
            "creds" => GetCredentialCommand(),
            "formentries" => GetFormEntriesCommand(),
            "logs" => GetLogCommand(),
            "markets" => GetMarketCommand(),
            "products" => GetProductCommand(),
            "sources" => GetSourceCommand(),
            "transactions" => GetTransactionCommand(),
            "txs" => GetTransactionCommand(),
            _ => throw new NotImplementedException($"Invalid command: {arg}")
        };

        private AccountCommand GetAccountCommand()
        {
            var accounts = _serviceProvider.GetRequiredService<IAccounts>();
            var sources = _serviceProvider.GetRequiredService<ISources>();
            var appSettings = _serviceProvider.GetRequiredService<IAppSettings>();
            return new AccountCommand(accounts, sources, appSettings);
        }

        public AppSettingCommand GetAppSettingsCommand()
        {
            var appSettings = _serviceProvider.GetRequiredService<IAppSettings>();
            return new AppSettingCommand(appSettings);
        }

        private CoinCommand GetCoinCommand()
        {
            var coins = _serviceProvider.GetRequiredService<ICoins>();
            return new CoinCommand(coins);
        }

        private CoinLookupCommand GetCoinLookupCommand()
        {
            var lookups = _serviceProvider.GetRequiredService<ICoinLookups>();
            var appSettings = _serviceProvider.GetRequiredService<IAppSettings>();
            return new CoinLookupCommand(lookups, appSettings);
        }

        private CredentialCommand GetCredentialCommand()
        {
            var credentials = _serviceProvider.GetRequiredService<ICredentials>();
            var appSettings = _serviceProvider.GetRequiredService<IAppSettings>();
            return new CredentialCommand(credentials, appSettings);
        }

        private FormEntriesCommand GetFormEntriesCommand()
        {
            var formEntries = _serviceProvider.GetRequiredService<IFormEntries>();
            var appSettings = _serviceProvider.GetRequiredService<IAppSettings>();
            return new FormEntriesCommand(formEntries, appSettings);
        }

        private LogCommand GetLogCommand()
        {
            var appSettings = _serviceProvider.GetRequiredService<IAppSettings>();
            return new LogCommand(appSettings);
        }

        private MarketCommand GetMarketCommand()
        {
            var markets = _serviceProvider.GetRequiredService<IMarkets>();
            var sources = _serviceProvider.GetRequiredService<ISources>();
            var appSettings = _serviceProvider.GetRequiredService<IAppSettings>();
            return new MarketCommand(markets, sources, appSettings);
        }

        private ProductCommand GetProductCommand()
        {
            var products = _serviceProvider.GetRequiredService<IProducts>();
            return new ProductCommand(products);
        }

        private SourceCommand GetSourceCommand()
        {
            var sources = _serviceProvider.GetRequiredService<ISources>();
            return new SourceCommand(sources);
        }

        private TransactionCommand GetTransactionCommand()
        {
            var transactions = _serviceProvider.GetRequiredService<ITransactions>();
            var appSettings = _serviceProvider.GetRequiredService<IAppSettings>();
            return new TransactionCommand(transactions, appSettings);
        }
    }
}
