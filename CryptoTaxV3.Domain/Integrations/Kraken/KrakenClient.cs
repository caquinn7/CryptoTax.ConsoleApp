using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Credentials;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json.Linq;

namespace CryptoTaxV3.Domain.Integrations.Kraken
{
    public class KrakenClient : IKrakenClient
    {
        private readonly IFlurlClient _client;
        private readonly ICredentials _creds;

        public KrakenClient(
            IFlurlClientFactory flurlClientFactory,
            ICredentials credentials)
        {
            _client = flurlClientFactory.Get("https://api.kraken.com");
            _creds = credentials;
        }

        public async Task<IEnumerable<KrakenLedgerEntryDto>> GetLedgerEntriesAsync()
        {
            int totalEntries;
            int offset = 0;
            var entries = new List<KrakenLedgerEntryDto>();
            do
            {
                JObject response = await QueryPrivateAsync("Ledgers", $"&type=all&ofs={offset}");

                var errors = (JArray)response["error"];
                if (errors.Any())
                {
                    throw new Exception(string.Join("\n", errors));
                }

                totalEntries = (int)response["result"]["count"];
                foreach (var kvp in (JObject)response["result"]["ledger"])
                {
                    var entry = kvp.Value.ToObject<KrakenLedgerEntryDto>();
                    entry.LedgerId = kvp.Key;
                    entries.Add(entry);
                }
                offset = entries.Count;

            } while (entries.Count < totalEntries);

            return entries;
        }

        private async Task<JObject> QueryPrivateAsync(string endpoint, string queryParameters)
        {
            long nonce = DateTime.Now.Ticks;
            queryParameters = "nonce=" + nonce + queryParameters;

            using var sha256 = SHA256.Create();
            byte[] queryBytes = Encoding.UTF8.GetBytes(nonce + queryParameters);
            byte[] queryBytesHashed = sha256.ComputeHash(queryBytes);

            string path = $"/0/private/{endpoint}";
            byte[] pathBytes = Encoding.UTF8.GetBytes(path);
            byte[] pathQueryBytes = new byte[pathBytes.Length + queryBytesHashed.Length];
            pathBytes.CopyTo(pathQueryBytes, 0);
            queryBytesHashed.CopyTo(pathQueryBytes, pathBytes.Length);

            string apiSecret = _creds.GetCredentialValue(TxSource.Kraken, "ApiSecret");
            string apiKey = _creds.GetCredentialValue(TxSource.Kraken, "ApiKey");

            using var hmacsha512 = new HMACSHA512(Convert.FromBase64String(apiSecret));
            byte[] signatureBytesHashed = hmacsha512.ComputeHash(pathQueryBytes);
            string signature = Convert.ToBase64String(signatureBytesHashed);

            string response = await _client
                .Request(path)
                .WithHeader("API-Key", apiKey)
                .WithHeader("API-Sign", signature)
                .PostUrlEncodedAsync(queryParameters)
                .ReceiveString();

            return JObject.Parse(response);
        }
    }
}
