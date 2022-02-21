using System.Collections.Generic;
using System.Linq;

namespace CryptoTaxV3.Domain.Transactions.DAL
{
    public class Transaction
    {
        public int Id { get; init; }
        public string Asset { get; set; }
        public decimal Quantity { get; init; }
        public decimal? UnitValue { get; set; }
        public long? UnitValueTimestamp { get; set; }
        public string Type { get; init; }
        public long Timestamp { get; init; }
        public string Source { get; set; }
        public string ExternalId { get; init; }
        public int? BatchId { get; init; }

        public bool IsDuplicate(IEnumerable<Transaction> txs) =>
            txs.Any(t => t.DupCheckVal == DupCheckVal);

        public string DupCheckVal => $"{Quantity}{Timestamp}{Type}{Asset}";
    }
}
