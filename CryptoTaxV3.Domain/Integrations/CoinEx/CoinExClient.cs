using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Credentials;
using CryptoTaxV3.Domain.Products;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CryptoTaxV3.Domain.Integrations.CoinEx
{
    public class CoinExClient : ICoinExClient
    {
        private readonly IFlurlClient _client;
        private readonly ICredentials _creds;

        public CoinExClient(
            IFlurlClientFactory flurlClientFactory,
            ICredentials credentials)
        {
            _client = flurlClientFactory.Get("https://api.coinex.com");
            _creds = credentials;
        }

        public async Task<IEnumerable<CoinExDealDto>> GetDealsAsync()
        {
            string accessId = _creds.GetCredentialValue(TxSource.CoinEx, "AccessId");
            string apiSecret = _creds.GetCredentialValue(TxSource.CoinEx, "ApiSecret");

            var queryParams = new QueryParams
            {
                access_id = accessId
            };

            var deals = new List<CoinExDealDto>();

            bool hasNext;
            do
            {
                queryParams.tonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var response = await _client
                    .Request("/v1/order/user/deals")
                    .SetQueryParams(queryParams)
                    .WithHeader("Authorization", GetRequestSignature(queryParams, apiSecret))
                    .GetJsonAsync<CoinExResponseDto<CoinExDealDto>>();

                if (response.Code != 0)
                {
                    throw new Exception($"CoinEx returned error code {response.Code}");
                }
                if (response.Data != null)
                {
                    deals.AddRange(response.Data.Data);
                }

                queryParams.page += 1;
                hasNext = response.Data.HasNext;

            } while (hasNext);

            return deals;
        }

        public async Task<IEnumerable<MarketDto>> GetMarketsAsync()
        {
            string response = await _client
                .Request("/v1/market/info")
                .GetStringAsync();

            var data = (JObject)JObject.Parse(response)["data"];
            var markets = new List<MarketDto>();
            foreach (var kvp in data)
            {
                markets.Add(new MarketDto
                {
                    Source = TxSource.CoinEx,
                    Base = kvp.Value["trading_name"].ToString(),
                    Quote = kvp.Value["pricing_name"].ToString()
                });
            }
            return markets;
        }

        private static string GetRequestSignature(QueryParams queryParams, string apiSecret) =>
            $"{queryParams}&secret_key={apiSecret}".ToMD5().ToUpper();

        private class QueryParams
        {
            public string access_id { get; set; }
            public int limit { get; } = 100;
            public string market { get; set; }
            public int page { get; set; } = 1;
            public long tonce { get; set; }

            public override string ToString()
            {
                var queryParamsList = GetType().GetProperties()
                    .Where(p => p.GetValue(this) != null)
                    .Select(p => $"{p.Name}={p.GetValue(this)}");

                return string.Join('&', queryParamsList);
            }
        }

        private class CoinExResponseDto<T>
        {
            public int Code { get; set; }
            public CoinExDataDto<T> Data { get; set; }
            public string Message { get; set; }
        }

        private class CoinExDataDto<T>
        {
            public IEnumerable<T> Data { get; set; }
            [JsonProperty("curr_page")]
            public int CurrPage { get; set; }
            [JsonProperty("has_next")]
            public bool HasNext { get; set; }
            public int Count { get; set; }
        }
    }
}
