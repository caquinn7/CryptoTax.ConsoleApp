namespace CryptoTaxV3.Domain.Integrations.Coinbase
{
    public class CoinbaseAccountDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Currency Currency { get; set; }
    }

    public class Currency
    {
        public string Code { get; set; }
    }
}
