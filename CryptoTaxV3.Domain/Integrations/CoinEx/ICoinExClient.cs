using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Products;

namespace CryptoTaxV3.Domain.Integrations.CoinEx
{
    public interface ICoinExClient
    {
        Task<IEnumerable<CoinExDealDto>> GetDealsAsync();
        Task<IEnumerable<MarketDto>> GetMarketsAsync();
    }
}
