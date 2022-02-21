using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Integrations.Coinbase;

namespace CryptoTaxV3.Domain.Products.Importers
{
    public class CoinbaseProMarketImporter : IMarketImporter
    {
        private readonly ICoinbaseProClient _coinbaseProClient;

        public CoinbaseProMarketImporter(ICoinbaseProClient coinbaseProClient)
        {
            _coinbaseProClient = coinbaseProClient;
        }

        public Task<IEnumerable<MarketDto>> GetMarketsAsync() => _coinbaseProClient.GetMarketsAsync();
    }
}
