﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Coins.DAL;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace CryptoTaxV3.Domain.Integrations.CoinPaprika
{
    public class CoinPaprikaClient : ICoinPaprikaClient
    {
        private readonly IFlurlClient _flurlClient;

        public CoinPaprikaClient(IFlurlClientFactory flurlClientFactory)
        {
            _flurlClient = flurlClientFactory.Get("https://api.coinpaprika.com");
        }

        public Task<IEnumerable<Coin>> GetCoinsAsync() =>
            _flurlClient
                .Request("/v1/coins")
                .GetJsonAsync<IEnumerable<Coin>>();

        public Task<IEnumerable<CoinPaprikaPriceDto>> GetHistoricalCoinDataAsync(string coinId, long start, string interval, int limit) =>
            _flurlClient
                .Request($"/v1/tickers/{coinId}/historical")
                .SetQueryParams(new { start, interval, limit })
                .GetJsonAsync<IEnumerable<CoinPaprikaPriceDto>>();
    }
}
