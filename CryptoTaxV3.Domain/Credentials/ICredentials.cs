using System.Collections.Generic;

namespace CryptoTaxV3.Domain.Credentials
{
    public interface ICredentials
    {
        IEnumerable<CredentialDto> Get();
        string GetCredentialValue(TxSource source, string credentialName);
        int ImportFromCsv(string filePath);
    }
}