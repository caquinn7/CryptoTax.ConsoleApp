using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Credentials;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;

namespace CryptoTaxV3.Domain.Integrations.Coinbase
{
    public class CoinbaseClient : ICoinbaseClient
    {
        private readonly IFlurlClient _client;
        private readonly ICredentials _creds;

        public CoinbaseClient(
            IFlurlClientFactory flurlClientFactory,
            ICredentials credentials)
        {
            _client = flurlClientFactory.Get("https://api.coinbase.com");
            _creds = credentials;
        }

        public Task<IEnumerable<CoinbaseAccountDto>> GetAccountsAsync() =>
            GetPaginatedDataAsync<CoinbaseAccountDto>("/v2/accounts");

        public Task<IEnumerable<CoinbaseTransactionDto>> GetTransactionsAsync(string accountId) =>
            GetPaginatedDataAsync<CoinbaseTransactionDto>($"/v2/accounts/{accountId}/transactions");

        private async Task<IEnumerable<T>> GetPaginatedDataAsync<T>(string resourcePath)
        {
            string apiSecret = _creds.GetCredentialValue(TxSource.Coinbase, "ApiSecret");
            string apiKey = _creds.GetCredentialValue(TxSource.Coinbase, "ApiKey");

            resourcePath += $"?limit={100}";
            var data = new List<T>();
            while (!string.IsNullOrWhiteSpace(resourcePath))
            {
                string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                string signature = $"{timestamp}GET{resourcePath}".ToSHA256(apiSecret);
                var response = await _client
                    .Request(resourcePath)
                    .WithHeader("CB-ACCESS-KEY", apiKey)
                    .WithHeader("CB-ACCESS-TIMESTAMP", timestamp)
                    .WithHeader("CB-ACCESS-SIGN", signature)
                    .GetJsonAsync<PaginatedResponse<T>>();
                data.AddRange(response.Data);
                resourcePath = response.Pagination.NextUri;
            }
            return data;
        }

        private class PaginatedResponse<T>
        {
            public Pagination Pagination { get; set; }
            public IEnumerable<T> Data { get; set; }
        }

        private class Pagination
        {
            [JsonProperty("next_uri")]
            public string NextUri { get; set; }
        }
    }
}
