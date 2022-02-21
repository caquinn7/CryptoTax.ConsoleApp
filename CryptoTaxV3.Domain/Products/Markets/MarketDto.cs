namespace CryptoTaxV3.Domain.Products
{
    public class MarketDto : ProductDto
    {
        public string Base { get; init; }
        public string Quote { get; init; }
        public string Name { get; init; }
    }
}
