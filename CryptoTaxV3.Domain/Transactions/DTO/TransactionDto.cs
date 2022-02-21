using System;

namespace CryptoTaxV3.Domain.Transactions
{
    public class TransactionDto
    {
        public int Id { get; init; }
        public string Asset { get; init; }
        public decimal Quantity { get; init; }
        public decimal? UnitValue { get; init; }
        public TxType Type { get; init; }
        public DateTime Timestamp { get; init; }
        public string Source { get; init; }
        public string ExternalId { get; init; }
    }
}