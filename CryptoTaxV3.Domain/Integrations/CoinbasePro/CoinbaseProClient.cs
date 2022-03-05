using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Credentials;
using CryptoTaxV3.Domain.Integrations.CoinbasePro;
using CryptoTaxV3.Domain.Products.DAL;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace CryptoTaxV3.Domain.Integrations.Coinbase
{
    public class CoinbaseProClient : ICoinbaseProClient
    {
        private readonly IFlurlClient _client;
        private readonly ICredentials _creds;

        public CoinbaseProClient(
            IFlurlClientFactory flurlClientFactory,
            ICredentials credentials)
        {
            _client = flurlClientFactory.Get("https://api.pro.coinbase.com");
            _creds = credentials;
        }

        public async Task<IEnumerable<CoinbaseProFillDto>> GetFillsAsync(string productId)
        {
            string apiSecret = _creds.GetCredentialValue(TxSource.CoinbasePro, "ApiSecret");
            string apiKey = _creds.GetCredentialValue(TxSource.CoinbasePro, "ApiKey");
            string passphrase = _creds.GetCredentialValue(TxSource.CoinbasePro, "Passphrase");

            var queryParams = new QueryParams
            {
                product_id = productId,
                limit = "100",
                after = null
            };
            var responseContent = new List<CoinbaseProFillDto>();
            var fills = new List<CoinbaseProFillDto>();
            do
            {
                string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                string signature = GetRequestSignature($"/fills{queryParams}", timestamp, apiSecret);

                var response = await _client
                    .Request("/fills")
                    .SetQueryParams(queryParams)
                    .WithHeader("User-Agent", "CryptoTaxCQ/1.0")
                    .WithHeader("CB-ACCESS-KEY", apiKey)
                    .WithHeader("CB-ACCESS-TIMESTAMP", timestamp)
                    .WithHeader("CB-ACCESS-SIGN", signature)
                    .WithHeader("CB-ACCESS-PASSPHRASE", passphrase)
                    .GetAsync();

                response.Headers.TryGetFirst("Cb-After", out string nextPageId);
                queryParams.after = nextPageId;

                responseContent = (await response.GetJsonAsync<IEnumerable<CoinbaseProFillDto>>()).ToList();
                fills.AddRange(responseContent);

            } while (responseContent.Any());

            return fills;
        }

        public async Task<IEnumerable<Market>> GetMarketsAsync()
        {
            IEnumerable<dynamic> response = await _client
                .WithHeader("User-Agent", "CryptoTaxCQ/1.0")
                .Request("/products")
                .GetJsonAsync<dynamic>();

            return response.Select(m => new Market
            {
                Source = TxSource.CoinbasePro.FastToString(),
                Base = m.base_currency,
                Quote = m.quote_currency
            });
        }

        private static string GetRequestSignature(string resourcePath, string timestamp, string apiSecret)
        {
            string signature = $"{timestamp}GET{resourcePath}";
            byte[] signatureBytes = Encoding.UTF8.GetBytes(signature);

            byte[] secretBytes = Convert.FromBase64String(apiSecret);
            using var hmac = new HMACSHA256(secretBytes);
            return Convert.ToBase64String(hmac.ComputeHash(signatureBytes));
        }

        private class QueryParams
        {
            public string product_id { get; set; }
            public string limit { get; set; }
            public string after { get; set; }

            public override string ToString()
            {
                var queryParamsList = GetType().GetProperties()
                    .Where(p => p.GetValue(this) != null)
                    .Select(p => $"{p.Name}={p.GetValue(this)}");

                return "?" + string.Join('&', queryParamsList);
            }
        }
    }
}
