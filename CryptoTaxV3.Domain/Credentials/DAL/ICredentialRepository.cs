using System.Collections.Generic;

namespace CryptoTaxV3.Domain.Credentials.DAL
{
    public interface ICredentialRepository
    {
        string GetCredentialValue(string source, string credentialName);
        IEnumerable<CredentialDto> GetCredentials();
        int Insert(IEnumerable<Credential> credentials);
    }
}