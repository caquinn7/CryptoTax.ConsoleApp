using System;
using CryptoTaxV3.Domain.Transactions.DAL;
using Newtonsoft.Json;

namespace CryptoTaxV3.Domain.Integrations.Coinbase
{
    public class CoinbaseTransactionDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public Money Amount { get; set; }
        [JsonProperty("native_amount")]
        public Money NativeAmount { get; set; }
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
        public Details Details { get; set; }

        public bool IsBuySellTradeOrEarn =>
            Type == "buy" || Type == "sell" || Type == "trade" || IsEarn;

        public bool IsEarn => Details.Subtitle.Contains("Coinbase Earn");

        public Transaction ToTransaction()
        {
            var quantity = decimal.Parse(Amount.Amount);
            var dollarValue = decimal.Parse(NativeAmount.Amount);
            var unitValue = decimal.Round(decimal.Divide(dollarValue, quantity), 2);

            TxType type = IsEarn
                ? TxType.Income
                : Enum.Parse<TxType>(Type, ignoreCase: true);

            var timestamp = DateTime.Parse(CreatedAt).ToUnixSeconds();

            return new Transaction
            {
                Asset = Amount.Currency.ToUpper(),
                Quantity = quantity,
                Type = type.FastToString(),
                Timestamp = timestamp,
                UnitValue = unitValue,
                UnitValueTimestamp = timestamp,
                Source = TxSource.Coinbase.FastToString(),
                ExternalId = Id,
            };
        }
    }

    public class Money
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
    }

    public class Details
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Header { get; set; }
    }
}
