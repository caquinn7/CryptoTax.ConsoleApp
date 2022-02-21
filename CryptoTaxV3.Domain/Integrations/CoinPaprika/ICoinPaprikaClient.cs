using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Coins.DAL;

namespace CryptoTaxV3.Domain.Integrations.CoinPaprika
{
    public interface ICoinPaprikaClient
    {
        Task<IEnumerable<Coin>> GetCoinsAsync();
        Task<IEnumerable<CoinPaprikaPriceDto>> GetHistoricalCoinDataAsync(string coinId, long start, string interval, int limit);
    }
}