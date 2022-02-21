using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Products;

namespace CryptoTaxV3.Domain.Integrations.Binance
{
    public interface IBinanceClient
    {
        Task<IEnumerable<BinanceOrderDto>> GetBinanceOrdersAsync(string market, bool isUS);
        Task<IEnumerable<MarketDto>> GetMarketsAsync(bool isUS);
    }
}