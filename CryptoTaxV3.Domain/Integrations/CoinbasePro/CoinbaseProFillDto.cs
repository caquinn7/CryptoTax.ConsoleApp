using System;
using System.Globalization;
using CryptoTaxV3.Domain.Transactions.DAL;
using Newtonsoft.Json;

namespace CryptoTaxV3.Domain.Integrations.CoinbasePro
{
    public class CoinbaseProFillDto
    {
        [JsonProperty("trade_id")]
        public int TradeId { get; set; }
        [JsonProperty("product_id")]
        public string ProductId { get; set; }
        public string Price { get; set; }
        public string Size { get; set; }
        [JsonProperty("order_id")]
        public string OrderId { get; set; }
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
        public string Liquidity { get; set; }
        public string Fee { get; set; }
        public bool Settled { get; set; }
        public string Side { get; set; }

        public (Transaction, Transaction) ToTransactions()
        {
            string type = TxType.Trade.FastToString();
            string source = TxSource.CoinbasePro.FastToString();
            string externalId = TradeId.ToString();

            string[] assets = ProductId.ToUpper().Split('-');
            var baseAsset = assets[0];
            var quoteAsset = assets[1];

            var baseQty = decimal.Parse(Size);
            var quoteQty = decimal.Parse(Price) * baseQty;
            if (Side == "buy")
            {
                quoteQty *= -1;
            }
            else
            {
                baseQty *= -1;
            }
            quoteQty -= decimal.Parse(Fee);

            var timestamp = DateTime
                .Parse(CreatedAt, null, DateTimeStyles.AssumeUniversal)
                .ToUnixSeconds();

            decimal? baseUnitValue = null;
            decimal? quoteUnitValue = null;
            if (baseAsset == "USD")
            {
                quoteUnitValue = Math.Abs(decimal.Round(decimal.Divide(baseQty, quoteQty), 2));
            }
            else if (quoteAsset == "USD")
            {
                baseUnitValue = Math.Abs(decimal.Round(decimal.Divide(quoteQty, baseQty), 2));
            }

            var baseTx = new Transaction
            {
                Asset = baseAsset,
                Quantity = baseQty,
                UnitValue = baseUnitValue,
                UnitValueTimestamp = baseUnitValue != null ? timestamp : null,
                Type = type,
                Timestamp = timestamp,
                Source = source,
                ExternalId = externalId,
            };
            var quoteTx = new Transaction
            {
                Asset = quoteAsset,
                Quantity = quoteQty,
                UnitValue = quoteUnitValue,
                UnitValueTimestamp = quoteUnitValue != null ? timestamp : null,
                Type = type,
                Timestamp = timestamp,
                Source = source,
                ExternalId = externalId,
            };
            return (baseTx, quoteTx);
        }
    }
}
