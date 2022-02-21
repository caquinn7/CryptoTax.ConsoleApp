namespace CryptoTaxV3.Domain.Products.DAL
{
    public class Account
    {
        public int Id { get; init; }
        public string Source { get; init; }
        public string Asset { get; init; }
        public string ExternalId { get; init; }
        public bool IsActive { get; init; }
    }
}
