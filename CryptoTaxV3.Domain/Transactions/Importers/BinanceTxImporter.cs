using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Integrations.Binance;
using CryptoTaxV3.Domain.Products;
using CryptoTaxV3.Domain.Products.DAL;
using CryptoTaxV3.Domain.Transactions.DAL;
using Microsoft.Extensions.Logging;

namespace CryptoTaxV3.Domain.Transactions.Importers
{
    public class BinanceTxImporter : BaseTxImporter, ITxImporter
    {
        private readonly IBinanceClient _binanceClient;
        private readonly IMarkets _markets;
        private readonly bool _isUS;

        public BinanceTxImporter(
            IBinanceClient binanceClient,
            IMarkets markets,
            ILogger<BinanceTxImporter> logger,
            bool isUS)
            : base(logger)
        {
            _binanceClient = binanceClient;
            _markets = markets;
            _isUS = isUS;
        }

        public async Task<ImportResult> GetTransactionsAsync()
        {
            var source = _isUS ? TxSource.BinanceUS : TxSource.Binance;
            var result = new ImportResult { Source = source };
            try
            {
                var markets = _markets.GetActive(source);
                var orderTasks = markets.Select(m => _binanceClient.GetBinanceOrdersAsync($"{m.Base}{m.Quote}", _isUS));
                var orderLists = await Task.WhenAll(orderTasks);
                result.Transactions = orderLists
                    .SelectMany(os => os.Where(o => o.Status == "FILLED"))
                    .Select(o => o.ToTransaction(GetMarket(o.Symbol), source))
                    .SelectMany(ts => new Transaction[] { ts.Item1, ts.Item2 });

                Market GetMarket(string symbol) => markets.Single(m => symbol == $"{m.Base}{m.Quote}");
            }
            catch (Exception ex)
            {
                var errId = LogError(ex, source);
                result.ErrorId = errId;
            }
            return result;
        }
    }
}
