namespace CryptoTaxV3.Domain.Credentials
{
    public class CredentialDto
    {
        public int Id { get; set; }
        public TxSource Source { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
