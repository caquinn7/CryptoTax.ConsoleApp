using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Integrations.Kraken;
using Microsoft.Extensions.Logging;

namespace CryptoTaxV3.Domain.Transactions.Importers
{
    public class KrakenTxImporter : BaseTxImporter, ITxImporter
    {
        private readonly IKrakenClient _krakenClient;

        public KrakenTxImporter(
            IKrakenClient krakenClient,
            ILogger<KrakenTxImporter> logger) : base(logger)
        {
            _krakenClient = krakenClient;
        }

        public async Task<ImportResult> GetTransactionsAsync()
        {
            var result = new ImportResult { Source = TxSource.Kraken };
            try
            {
                IEnumerable<KrakenLedgerEntryDto> ledgerEntries = await _krakenClient.GetLedgerEntriesAsync();
                result.Transactions = ledgerEntries
                    .Where(e => e.Type == "receive" || e.Type == "sale" || e.Type == "trade")
                    .Select(e => e.ToTransaction());
            }
            catch (Exception ex)
            {
                var errId = LogError(ex, TxSource.Kraken);
                result.ErrorId = errId;
            }
            return result;
        }
    }
}
