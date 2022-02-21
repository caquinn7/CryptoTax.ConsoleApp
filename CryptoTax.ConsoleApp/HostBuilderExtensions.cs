using System;
using System.Data.SQLite;
using CryptoTax.ConsoleApp.Application;
using CryptoTax.ConsoleApp.Commands.Factory;
using CryptoTax.ConsoleApp.Database;
using CryptoTaxV3.Domain.AppSettings;
using CryptoTaxV3.Domain.AppSettings.DAL;
using CryptoTaxV3.Domain.CoinLookups;
using CryptoTaxV3.Domain.CoinLookups.DAL;
using CryptoTaxV3.Domain.Coins;
using CryptoTaxV3.Domain.Coins.DAL;
using CryptoTaxV3.Domain.Credentials;
using CryptoTaxV3.Domain.Credentials.DAL;
using CryptoTaxV3.Domain.FormEntries;
using CryptoTaxV3.Domain.Infrastructure.Csv;
using CryptoTaxV3.Domain.Integrations.Binance;
using CryptoTaxV3.Domain.Integrations.Coinbase;
using CryptoTaxV3.Domain.Integrations.CoinEx;
using CryptoTaxV3.Domain.Integrations.CoinPaprika;
using CryptoTaxV3.Domain.Integrations.Kraken;
using CryptoTaxV3.Domain.Products;
using CryptoTaxV3.Domain.Products.DAL;
using CryptoTaxV3.Domain.Products.Importers;
using CryptoTaxV3.Domain.Services.CoinData;
using CryptoTaxV3.Domain.Sources;
using CryptoTaxV3.Domain.Sources.DAL;
using CryptoTaxV3.Domain.Transactions;
using CryptoTaxV3.Domain.Transactions.DAL;
using CryptoTaxV3.Domain.Transactions.Importers;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CryptoTax.ConsoleApp
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder RegisterDependencies(this IHostBuilder hostBuilder, IConfiguration configuration)
        {
            string connStr = configuration.GetConnectionString("Default");
            return hostBuilder.ConfigureServices((_, services) =>
            {
                services.AddTransient<IAccountRepository>(p => new AccountRepository(connStr));
                services.AddTransient<IAppSettingRepository>(p => new AppSettingRepository(connStr));
                services.AddTransient<ICoinRepository>(p => new CoinRepository(connStr));
                services.AddTransient<ICoinLookupRepository>(p => new CoinLookupRepository(connStr));
                services.AddTransient<ICredentialRepository>(p => new CredentialRepository(connStr));
                services.AddTransient<IMarketRepository>(p => new MarketRepository(connStr));
                services.AddTransient<ISourceRepository>(p => new SourceRepository(connStr));
                services.AddTransient<ITransactionRepository>(p => new TransactionRepository(connStr));

                services.AddTransient<IAccounts, Accounts>();
                services.AddTransient<IAppSettings, AppSettings>();
                services.AddTransient<ICredentials, Credentials>();
                services.AddTransient<ICoinLookups, CoinLookups>();
                services.AddTransient<ICoins, Coins>();
                services.AddTransient<IFormEntries, FormEntries>();
                services.AddTransient<IMarkets, Markets>();
                services.AddTransient<IProducts, Products>();
                services.AddTransient<ISources, Sources>();
                services.AddTransient<ITransactions, Transactions>();

                services.AddTransient<IProductImporter, ProductImporter>();
                services.AddTransient<IAccountImporter, CoinbaseAccountImporter>();
                services.AddTransient<IMarketImporterFactory, MarketImporterFactory>();

                services.AddSingleton<IFlurlClientFactory, CustomFlurlClientFactory>();
                services.AddTransient<IBinanceClient, BinanceClient>();
                services.AddTransient<ICoinbaseClient, CoinbaseClient>();
                services.AddTransient<ICoinbaseProClient, CoinbaseProClient>();
                services.AddTransient<ICoinExClient, CoinExClient>();
                services.AddTransient<ICoinPaprikaClient, CoinPaprikaClient>();
                services.AddTransient<IKrakenClient, KrakenClient>();

                services.AddTransient<ICoinDataService, CoinDataService>();

                services.AddSingleton<ITxImporterFactory, TxImporterFactory>();
                services.AddSingleton<ICsvReaderWrapper, CsvReaderWrapper>();
                services.AddSingleton<ICommandFactory, CommandFactory>();
                services.AddSingleton<App>();
            });
        }

        public static IHostBuilder SetupDatabase(this IHostBuilder hostBuilder, IConfiguration configuration)
        {
            string connStr = configuration.GetConnectionString("Default");
            string env = Environment.GetEnvironmentVariable("ENVIRONMENT");

            using (var conn = new SQLiteConnection(connStr))
            {
                var dbSeeder = new DbSeeder(conn);
                using (var tx = conn.BeginTransaction())
                {
                    if (env == "Development")
                    {
                        dbSeeder.DropTables();
                    }
                    dbSeeder.CreateTables();

                    dbSeeder.InsertSources();
                    dbSeeder.InsertTransactionTypes();
                    tx.Commit();
                }
            }

            return hostBuilder;
        }
    }
}
