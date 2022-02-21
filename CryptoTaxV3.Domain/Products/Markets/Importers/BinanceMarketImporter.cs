using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Integrations.Binance;

namespace CryptoTaxV3.Domain.Products.Importers.Markets
{
    public class BinanceMarketImporter : IMarketImporter
    {
        private readonly IBinanceClient _binanceClient;
        private readonly bool _isUS;

        public BinanceMarketImporter(IBinanceClient binanceClient, bool isUS)
        {
            _binanceClient = binanceClient;
            _isUS = isUS;
        }

        public Task<IEnumerable<MarketDto>> GetMarketsAsync() => _binanceClient.GetMarketsAsync(_isUS);
    }
}
