using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Credentials;
using CryptoTaxV3.Domain.Products;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json.Linq;

namespace CryptoTaxV3.Domain.Integrations.Binance
{
    public class BinanceClient : IBinanceClient
    {
        private readonly IFlurlClientFactory _clientFactory;
        private readonly ICredentials _creds;

        public BinanceClient(
            IFlurlClientFactory flurlClientFactory,
            ICredentials credentials)
        {
            _clientFactory = flurlClientFactory;
            _creds = credentials;
        }

        public async Task<IEnumerable<BinanceOrderDto>> GetBinanceOrdersAsync(string market, bool isUS)
        {
            var source = isUS ? TxSource.BinanceUS : TxSource.Binance;

            string apiSecret = _creds.GetCredentialValue(source, "ApiSecret");
            string apiKey = _creds.GetCredentialValue(source, "ApiKey");

            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string queryString = $"symbol={market}&timestamp={timestamp}";
            string signature = queryString.ToSHA256(apiSecret);

            queryString += $"&signature={signature}";

            return await GetClient(isUS)
                .Request("/api/v3/allOrders?" + queryString)
                .WithHeader("X-MBX-APIKEY", apiKey)
                .GetJsonAsync<IEnumerable<BinanceOrderDto>>();
        }

        public async Task<IEnumerable<MarketDto>> GetMarketsAsync(bool isUS)
        {
            string response = await GetClient(isUS)
                .Request("/api/v3/exchangeInfo")
                .GetStringAsync();

            return JObject.Parse(response)["symbols"].Select(s => new MarketDto
            {
                Source = isUS ? TxSource.BinanceUS : TxSource.Binance,
                Base = s["baseAsset"].ToString(),
                Quote = s["quoteAsset"].ToString()
            }).ToList();
        }

        private IFlurlClient GetClient(bool isUS) =>
            _clientFactory.Get($"https://api.binance.{(isUS ? "us" : "com")}");
    }
}
