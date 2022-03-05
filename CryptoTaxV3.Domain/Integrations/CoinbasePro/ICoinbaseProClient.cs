using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Integrations.CoinbasePro;
using CryptoTaxV3.Domain.Products.DAL;

namespace CryptoTaxV3.Domain.Integrations.Coinbase
{
    public interface ICoinbaseProClient
    {
        Task<IEnumerable<CoinbaseProFillDto>> GetFillsAsync(string productId);
        Task<IEnumerable<Market>> GetMarketsAsync();
    }
}
