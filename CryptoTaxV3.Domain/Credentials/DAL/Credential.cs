namespace CryptoTaxV3.Domain.Credentials.DAL
{
    public class Credential
    {
        public int Id { get; init; }
        public string Source { get; init; }
        public string Name { get; init; }
        public string Value { get; init; }
    }
}
