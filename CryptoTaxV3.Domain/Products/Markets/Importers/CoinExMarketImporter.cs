using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Integrations.CoinEx;
using CryptoTaxV3.Domain.Products.DAL;

namespace CryptoTaxV3.Domain.Products.Importers
{
    public class CoinExMarketImporter : IMarketImporter
    {
        private readonly ICoinExClient _coinExClient;

        public CoinExMarketImporter(ICoinExClient coinExClient)
        {
            _coinExClient = coinExClient;
        }

        public Task<IEnumerable<Market>> GetMarketsAsync() => _coinExClient.GetMarketsAsync();
    }
}
