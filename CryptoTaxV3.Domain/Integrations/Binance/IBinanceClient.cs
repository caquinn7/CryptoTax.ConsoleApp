using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Products.DAL;

namespace CryptoTaxV3.Domain.Integrations.Binance
{
    public interface IBinanceClient
    {
        Task<IEnumerable<BinanceOrderDto>> GetBinanceOrdersAsync(string market, bool isUS);
        Task<IEnumerable<Market>> GetMarketsAsync(bool isUS);
    }
}