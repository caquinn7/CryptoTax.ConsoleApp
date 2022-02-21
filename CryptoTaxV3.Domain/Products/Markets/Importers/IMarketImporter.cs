using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoTaxV3.Domain.Products.Importers
{
    public interface IMarketImporter
    {
        Task<IEnumerable<MarketDto>> GetMarketsAsync();
    }
}
