using System;

namespace CryptoTaxV3.Domain
{
    public enum ImportType
    {
        Csv,
        Integration
    }

    public enum ProductType
    {
        Account,
        Market
    }

    public enum TxSource
    {
        Binance = 1,
        BinanceUS = 2,
        Coinbase = 3,
        CoinbasePro = 4,
        Kraken = 5,
        CoinEx = 6
    }

    public enum TxType
    {
        Buy,
        Fork,
        Gift,
        Income,
        Lost,
        Mining,
        NetworkFee,
        Payment,
        Sell,
        Trade,
        TradingFee
    }

    static class ProductTypeExtenstions
    {
        public static string FastToString(this ProductType productType) => productType switch
        {
            ProductType.Account => nameof(ProductType.Account),
            ProductType.Market => nameof(ProductType.Market),
            _ => throw new ArgumentOutOfRangeException(nameof(productType), productType, null)
        };
    }

    static class TxSourceExtensions
    {
        public static string FastToString(this TxSource source) => source switch
        {
            TxSource.Binance => nameof(TxSource.Binance),
            TxSource.BinanceUS => nameof(TxSource.BinanceUS),
            TxSource.Coinbase => nameof(TxSource.Coinbase),
            TxSource.CoinbasePro => nameof(TxSource.CoinbasePro),
            TxSource.Kraken => nameof(TxSource.Kraken),
            TxSource.CoinEx => nameof(TxSource.CoinEx),
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }

    static class TxTypeExtensions
    {
        public static string FastToString(this TxType type) => type switch
        {
            TxType.Buy => nameof(TxType.Buy),
            TxType.Fork => nameof(TxType.Fork),
            TxType.Gift => nameof(TxType.Gift),
            TxType.Income => nameof(TxType.Income),
            TxType.Lost => nameof(TxType.Lost),
            TxType.Mining => nameof(TxType.Mining),
            TxType.NetworkFee => nameof(TxType.NetworkFee),
            TxType.Payment => nameof(TxType.Payment),
            TxType.Sell => nameof(TxType.Sell),
            TxType.Trade => nameof(TxType.Trade),
            TxType.TradingFee => nameof(TxType.TradingFee),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
