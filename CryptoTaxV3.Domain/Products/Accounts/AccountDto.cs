namespace CryptoTaxV3.Domain.Products
{
    public class AccountDto : ProductDto
    {
        public string Asset { get; init; }
        public string ExternalId { get; init; }
    }
}
