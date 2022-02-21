using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Integrations.Coinbase;
using CryptoTaxV3.Domain.Products;
using Microsoft.Extensions.Logging;

namespace CryptoTaxV3.Domain.Transactions.Importers
{
    public class CoinbaseTxImporter : BaseTxImporter, ITxImporter
    {
        private readonly ICoinbaseClient _coinbaseClient;
        private readonly IAccounts _accounts;

        public CoinbaseTxImporter(
            ICoinbaseClient coinbaseClient,
            IAccounts accounts,
            ILogger<CoinbaseTxImporter> logger) : base(logger)
        {
            _coinbaseClient = coinbaseClient;
            _accounts = accounts;
        }

        public async Task<ImportResult> GetTransactionsAsync()
        {
            var result = new ImportResult { Source = TxSource.Coinbase };
            try
            {
                result.Transactions = (await GetCoinbaseTransactionsAsync())
                    .Where(t => t.IsBuySellTradeOrEarn)
                    .Select(t => t.ToTransaction());
            }
            catch (Exception ex)
            {
                var errId = LogError(ex, TxSource.Coinbase);
                result.ErrorId = errId;
            }
            return result;
        }

        private async Task<IEnumerable<CoinbaseTransactionDto>> GetCoinbaseTransactionsAsync()
        {
            var accounts = _accounts.GetActive(TxSource.Coinbase);
            var txLists = await Task.WhenAll(accounts.Select(a => _coinbaseClient.GetTransactionsAsync(a.ExternalId)));
            return txLists.SelectMany(t => t);
        }
    }
}
