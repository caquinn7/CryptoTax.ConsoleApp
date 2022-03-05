namespace CryptoTaxV3.Domain.Sources
{
    public class SourceDto
    {
        public TxSource Name { get; init; }
        public string ProductType { get; init; }
        public bool IsActive { get; init; }
    }
}
