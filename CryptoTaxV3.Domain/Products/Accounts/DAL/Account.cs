namespace CryptoTaxV3.Domain.Products.DAL
{
    public class Account : Product
    {
        public string Asset { get; init; }
        public string ExternalId { get; init; }
    }
}
