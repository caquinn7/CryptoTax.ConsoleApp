using System;
using CryptoTaxV3.Domain.Transactions.DAL;

namespace CryptoTaxV3.Domain.Integrations.Kraken
{
    public class KrakenLedgerEntryDto
    {
        public string LedgerId { get; set; }
        public string RefId { get; set; }
        public decimal Time { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public string AClass { get; set; }
        public string Asset { get; set; }
        public string Amount { get; set; }
        public string Fee { get; set; }
        public string Balance { get; set; }

        public Transaction ToTransaction() => new()
        {
            Asset = Asset,
            Quantity = decimal.Parse(Amount),
            Type = TxType.Trade.FastToString(),
            Timestamp = Convert.ToInt64(Time),
            Source = TxSource.Kraken.FastToString(),
            ExternalId = RefId,
        };
    }
}
