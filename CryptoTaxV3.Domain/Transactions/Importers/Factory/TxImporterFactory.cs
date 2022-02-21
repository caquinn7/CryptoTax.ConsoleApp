using System;
using CryptoTaxV3.Domain.Integrations.Binance;
using CryptoTaxV3.Domain.Integrations.Coinbase;
using CryptoTaxV3.Domain.Integrations.CoinEx;
using CryptoTaxV3.Domain.Integrations.Kraken;
using CryptoTaxV3.Domain.Products;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CryptoTaxV3.Domain.Transactions.Importers
{
    public class TxImporterFactory : ITxImporterFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoggerFactory _loggerFactory;

        public TxImporterFactory(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _serviceProvider = serviceProvider;
            _loggerFactory = loggerFactory;
        }

        public ITxImporter GetImporter(TxSource source) => source switch
        {
            TxSource.Binance => GetBinanceImporter(isUS: false),
            TxSource.BinanceUS => GetBinanceImporter(isUS: true),
            TxSource.Coinbase => GetCoinbaseImporter(),
            TxSource.CoinbasePro => GetCoinbaseProImporter(),
            TxSource.CoinEx => GetCoinExImporter(),
            TxSource.Kraken => GetKrakenImporter(),
            _ => throw new NotImplementedException()
        };

        private BinanceTxImporter GetBinanceImporter(bool isUS)
        {
            var client = _serviceProvider.GetService<IBinanceClient>();
            var markets = _serviceProvider.GetService<IMarkets>();
            var logger = _loggerFactory.CreateLogger<BinanceTxImporter>();
            return new BinanceTxImporter(client, markets, logger, isUS);
        }

        private CoinbaseTxImporter GetCoinbaseImporter()
        {
            var client = _serviceProvider.GetService<ICoinbaseClient>();
            var accounts = _serviceProvider.GetService<IAccounts>();
            var logger = _loggerFactory.CreateLogger<CoinbaseTxImporter>();
            return new CoinbaseTxImporter(client, accounts, logger);
        }

        private CoinbaseProTxImporter GetCoinbaseProImporter()
        {
            var client = _serviceProvider.GetService<ICoinbaseProClient>();
            var markets = _serviceProvider.GetService<IMarkets>();
            var logger = _loggerFactory.CreateLogger<CoinbaseProTxImporter>();
            return new CoinbaseProTxImporter(client, markets, logger);
        }

        private CoinExTxImporter GetCoinExImporter()
        {
            var client = _serviceProvider.GetService<ICoinExClient>();
            var markets = _serviceProvider.GetService<IMarkets>();
            var logger = _loggerFactory.CreateLogger<CoinExTxImporter>();
            return new CoinExTxImporter(client, markets, logger);
        }

        private KrakenTxImporter GetKrakenImporter()
        {
            var client = _serviceProvider.GetService<IKrakenClient>();
            var logger = _loggerFactory.CreateLogger<KrakenTxImporter>();
            return new KrakenTxImporter(client, logger);
        }
    }
}
