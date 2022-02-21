using System.Threading.Tasks;

namespace CryptoTaxV3.Domain.Services.CoinData
{
    public interface ICoinDataService
    {
        Task<(decimal?, long?)> GetHistoricalPriceAsync(string asset, long unixTimeSeconds);
        string GetStandardSymbol(string symbol);
    }
}