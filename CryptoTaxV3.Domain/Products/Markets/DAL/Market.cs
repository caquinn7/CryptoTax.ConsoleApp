namespace CryptoTaxV3.Domain.Products.DAL
{
    public class Market
    {
        public int Id { get; init; }
        public string Source { get; init; }
        public string Base { get; init; }
        public string Quote { get; init; }
        public bool IsActive { get; init; }
    }
}