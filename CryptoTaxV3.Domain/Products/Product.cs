namespace CryptoTaxV3.Domain.Products
{
    public abstract class Product
    {
        public int Id { get; init; }
        public string Source { get; init; }
        public bool IsActive { get; init; }
    }
}
