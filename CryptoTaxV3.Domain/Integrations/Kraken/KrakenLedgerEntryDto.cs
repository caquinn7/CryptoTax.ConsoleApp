using System;
using CryptoTaxV3.Domain.Exceptions;
using CryptoTaxV3.Domain.Transactions.DAL;

namespace CryptoTaxV3.Domain.Integrations.Kraken
{
    public class KrakenLedgerEntryDto
    {
        public string LedgerId { get; set; }
        public string RefId { get; init; }
        public decimal Time { get; init; }
        public string Type { get; init; }
        public string Subtype { get; init; }
        public string AClass { get; init; }
        public string Asset { get; init; }
        public string Amount { get; set; }
        public string Fee { get; init; }
        public string Balance { get; init; }

        public Transaction ToTransaction() => new()
        {
            Asset = Asset,
            Quantity = decimal.Parse(Amount),
            Type = GetTxType().FastToString(),
            Timestamp = Convert.ToInt64(Time),
            Source = TxSource.Kraken.FastToString(),
            ExternalId = RefId,
        };

        public TxType GetTxType() => Type switch
        {
            "receive" => TxType.Buy,
            "sale" => TxType.Sell,
            "trade" => TxType.Trade,
            _ => throw new ValidationException($"Unhandled ledger entry type: {Type}")
        };
    }
}
