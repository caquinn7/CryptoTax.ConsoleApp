using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Integrations.Coinbase;
using CryptoTaxV3.Domain.Products;
using CryptoTaxV3.Domain.Transactions.DAL;
using Microsoft.Extensions.Logging;

namespace CryptoTaxV3.Domain.Transactions.Importers
{
    public class CoinbaseProTxImporter : BaseTxImporter, ITxImporter
    {
        private readonly ICoinbaseProClient _coinbaseProClient;
        private readonly IMarkets _markets;

        public CoinbaseProTxImporter(
            ICoinbaseProClient coinbaseProClient,
            IMarkets markets,
            ILogger<CoinbaseProTxImporter> logger) : base(logger)
        {
            _coinbaseProClient = coinbaseProClient;
            _markets = markets;
        }

        public async Task<ImportResult> GetTransactionsAsync()
        {
            var result = new ImportResult { Source = TxSource.CoinbasePro };
            try
            {
                var markets = _markets.GetActive(TxSource.CoinbasePro);
                var fillTasks = markets.Select(m => _coinbaseProClient.GetFillsAsync($"{m.Base}-{m.Quote}"));
                var fills = await Task.WhenAll(fillTasks);
                result.Transactions = fills
                    .SelectMany(fs => fs.Select(f => f.ToTransactions()))
                    .SelectMany(fs => (new Transaction[] { fs.Item1, fs.Item2 }));
            }
            catch (Exception ex)
            {
                var errId = LogError(ex, TxSource.CoinbasePro);
                result.ErrorId = errId;
            }
            return result;
        }
    }
}
