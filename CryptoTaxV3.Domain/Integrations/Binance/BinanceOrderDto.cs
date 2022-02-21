using CryptoTaxV3.Domain.Products.DAL;
using CryptoTaxV3.Domain.Transactions.DAL;

namespace CryptoTaxV3.Domain.Integrations.Binance
{
    public class BinanceOrderDto
    {
        public string Symbol { get; set; }
        public int OrderId { get; set; }
        public string Price { get; set; }
        public string OrigQty { get; set; }
        public string ExecutedQty { get; set; }
        public string CummulativeQuoteQty { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Side { get; set; }
        public long UpdateTime { get; set; }
        public string OrigQuoteOrderQty { get; set; }

        public (Transaction, Transaction) ToTransaction(Market market, TxSource source)
        {
            var baseQty = decimal.Parse(ExecutedQty);
            var quoteQty = decimal.Parse(CummulativeQuoteQty);
            if (Side == "BUY") quoteQty *= -1;
            else baseQty *= -1;

            long timestampUnixSeconds = UpdateTime / 1000;
            string sourceTxId = OrderId.ToString();

            var baseTx = new Transaction
            {
                Asset = market.Base.ToUpper(),
                Quantity = baseQty,
                Type = TxType.Trade.FastToString(),
                Timestamp = timestampUnixSeconds,
                Source = source.ToString(),
                ExternalId = sourceTxId
            };
            var quoteTx = new Transaction
            {
                Asset = market.Quote.ToUpper(),
                Quantity = quoteQty,
                Type = TxType.Trade.FastToString(),
                Timestamp = timestampUnixSeconds,
                Source = source.ToString(),
                ExternalId = sourceTxId
            };
            return (baseTx, quoteTx);
        }
    }
}
