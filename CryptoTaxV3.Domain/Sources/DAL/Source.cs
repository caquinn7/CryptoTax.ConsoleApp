namespace CryptoTaxV3.Domain.Sources.DAL
{
    public class Source
    {
        public string Name { get; init; }
        public string ProductType { get; init; }
        public bool? ProductHyphenated { get; init; }
        public bool IsActive { get; init; }
    }
}
