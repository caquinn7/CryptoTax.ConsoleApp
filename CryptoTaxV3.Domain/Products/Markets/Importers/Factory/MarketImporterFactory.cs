using System;
using CryptoTaxV3.Domain.Integrations.Binance;
using CryptoTaxV3.Domain.Integrations.Coinbase;
using CryptoTaxV3.Domain.Integrations.CoinEx;
using CryptoTaxV3.Domain.Products.Importers.Markets;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoTaxV3.Domain.Products.Importers
{
    public class MarketImporterFactory : IMarketImporterFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public MarketImporterFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IMarketImporter GetImporter(TxSource source) => source switch
        {
            TxSource.Binance => GetBinanceImporter(isUS: false),
            TxSource.BinanceUS => GetBinanceImporter(isUS: true),
            TxSource.CoinbasePro => GetCoinbaseProMarketImporter(),
            TxSource.CoinEx => GetCoinExMarketImporter(),
            _ => throw new NotImplementedException()
        };

        private BinanceMarketImporter GetBinanceImporter(bool isUS)
        {
            var client = _serviceProvider.GetService<IBinanceClient>();
            return new BinanceMarketImporter(client, isUS);
        }

        private CoinbaseProMarketImporter GetCoinbaseProMarketImporter()
        {
            var client = _serviceProvider.GetService<ICoinbaseProClient>();
            return new CoinbaseProMarketImporter(client);
        }

        private CoinExMarketImporter GetCoinExMarketImporter()
        {
            var client = _serviceProvider.GetService<ICoinExClient>();
            return new CoinExMarketImporter(client);
        }
    }
}
