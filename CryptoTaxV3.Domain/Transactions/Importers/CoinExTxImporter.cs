using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Integrations.CoinEx;
using CryptoTaxV3.Domain.Products;
using CryptoTaxV3.Domain.Products.DAL;
using CryptoTaxV3.Domain.Transactions.DAL;
using Microsoft.Extensions.Logging;

namespace CryptoTaxV3.Domain.Transactions.Importers
{
    public class CoinExTxImporter : BaseTxImporter, ITxImporter
    {
        private readonly ICoinExClient _coinExClient;
        private readonly IMarkets _markets;

        public CoinExTxImporter(
            ICoinExClient coinExClient,
            IMarkets markets,
            ILogger<CoinExTxImporter> logger) : base(logger)
        {
            _coinExClient = coinExClient;
            _markets = markets;
        }

        public async Task<ImportResult> GetTransactionsAsync()
        {
            var result = new ImportResult { Source = TxSource.CoinEx };
            try
            {
                var dealsTask = _coinExClient.GetDealsAsync();
                var markets = _markets.GetActive(TxSource.CoinEx);

                IEnumerable<CoinExDealDto> deals = await dealsTask;
                result.Transactions = deals
                    .Select(d => d.ToTransactions(GetMarket(d.Market)))
                    .SelectMany(ts => (new Transaction[] { ts.Item1, ts.Item2, ts.Item3 }))
                    .Where(t => t is not null);

                Market GetMarket(string symbol) => markets.Single(m => symbol == $"{m.Base}{m.Quote}");
            }
            catch (Exception ex)
            {
                var errId = LogError(ex, TxSource.CoinEx);
                result.ErrorId = errId;
            }
            return result;
        }
    }
}
