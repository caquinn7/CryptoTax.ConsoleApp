using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.CoinLookups;
using CryptoTaxV3.Domain.CoinLookups.DAL;
using CryptoTaxV3.Domain.Coins;
using CryptoTaxV3.Domain.Coins.DAL;
using CryptoTaxV3.Domain.Integrations.CoinPaprika;

namespace CryptoTaxV3.Domain.Services.CoinData
{
    public class CoinDataService : ICoinDataService
    {
        private readonly ICoins _coins;
        private readonly ICoinLookups _lookups;
        private readonly ICoinPaprikaClient _coinPaprikaClient;

        public CoinDataService(
            ICoins coins,
            ICoinLookups coinLookups,
            ICoinPaprikaClient coinPaprikaClient)
        {
            _coins = coins;
            _lookups = coinLookups;
            _coinPaprikaClient = coinPaprikaClient;
        }

        public string GetStandardSymbol(string symbol)
        {
            CoinLookup coinLookup = _lookups.Get(symbol);
            if (coinLookup == null) return null;

            Coin coin = _coins.Get(coinLookup.CoinId);
            return coin.Symbol;
        }

        public async Task<(decimal?, long?)> GetHistoricalPriceAsync(string asset, long unixTimeSeconds)
        {
            string coinId = _lookups.GetCoinId(asset);
            if (string.IsNullOrWhiteSpace(coinId)) return default;

            var coinDatas = await _coinPaprikaClient.GetHistoricalCoinDataAsync(coinId, unixTimeSeconds, "5m", 1);
            CoinPaprikaPriceDto coinData = coinDatas.FirstOrDefault();
            if (coinData == null) return default;

            long priceTimestamp = DateTimeOffset
                .Parse(coinData.Timestamp, null, DateTimeStyles.AssumeUniversal)
                .ToUnixTimeSeconds();

            return (coinData.Price, priceTimestamp);
        }
    }
}
