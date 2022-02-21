using CryptoTaxV3.Domain.Products.DAL;
using CryptoTaxV3.Domain.Transactions.DAL;
using Newtonsoft.Json;

namespace CryptoTaxV3.Domain.Integrations.CoinEx
{
    public class CoinExDealDto
    {
        public long Id { get; set; }
        [JsonProperty("order_id")]
        public long OrderId { get; set; }
        [JsonProperty("create_time")]
        public long CreateTime { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        [JsonProperty("fee_asset")]
        public string FeeAsset { get; set; }
        public string Market { get; set; }
        [JsonProperty("deal_money")]
        public decimal DealMoney { get; set; } // executed value

        public (Transaction, Transaction, Transaction) ToTransactions(Market market)
        {
            string type = TxType.Trade.FastToString();
            string source = TxSource.CoinEx.FastToString();
            string externalId = OrderId.ToString();

            decimal baseQty = Amount;
            decimal quoteQty = DealMoney;
            if (Type == "buy") quoteQty *= -1;
            else baseQty *= -1;

            var baseTx = new Transaction
            {
                Asset = market.Base,
                Quantity = baseQty,
                Type = type,
                Timestamp = CreateTime,
                Source = source,
                ExternalId = externalId
            };
            var quoteTx = new Transaction
            {
                Asset = market.Quote,
                Quantity = quoteQty,
                Type = type,
                Timestamp = CreateTime,
                Source = source,
                ExternalId = externalId
            };

            Transaction feeTx = null;
            if (FeeAsset != market.Base && FeeAsset != market.Quote)
            {
                feeTx = new Transaction
                {
                    Asset = FeeAsset,
                    Quantity = -Fee,
                    Type = TxType.TradingFee.FastToString(),
                    Timestamp = CreateTime,
                    Source = source,
                    ExternalId = externalId
                };
            }

            return (baseTx, quoteTx, feeTx);
        }
    }
}
