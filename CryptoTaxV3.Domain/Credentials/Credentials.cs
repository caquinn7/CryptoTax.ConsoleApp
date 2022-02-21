using System.Collections.Generic;
using System.Linq;
using CryptoTaxV3.Domain.Credentials.DAL;
using CryptoTaxV3.Domain.Infrastructure;
using CryptoTaxV3.Domain.Infrastructure.Csv;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace CryptoTaxV3.Domain.Credentials
{
    public class Credentials : ICredentials
    {
        private readonly ICredentialRepository _credRepo;
        private readonly ICsvReaderWrapper _csvReader;

        public Credentials(
            ICredentialRepository credentialRepository,
            ICsvReaderWrapper csvReader)
        {
            _credRepo = credentialRepository;
            _csvReader = csvReader;
        }

        public IEnumerable<CredentialDto> Get() => _credRepo.GetCredentials();

        public string GetCredentialValue(TxSource source, string credentialName)
        {
            Preconditions.ThrowValidationIfNullOrWhiteSpace(credentialName, "Credential Name is required");

            string cred = _credRepo.GetCredentialValue(source.ToString(), credentialName.Trim());
            if (string.IsNullOrWhiteSpace(cred))
            {
                throw new ConfigurationException($@"Credential ""{credentialName}"" not found for {source}");
            }
            return cred;
        }

        public int ImportFromCsv(string filePath)
        {
            var credDtos = _csvReader.GetRecords(filePath, new CredentialMap());
            var creds = credDtos.Select(c => new Credential
            {
                Source = c.Source.ToString(),
                Name = c.Name,
                Value = c.Value
            });
            foreach (var cred in creds)
            {
                Preconditions.ThrowValidationIfNullOrWhiteSpace(cred.Name, "Credential Name required");
            }
            return _credRepo.Insert(creds);
        }
    }

    internal class CredentialMap : ClassMap<CredentialDto>
    {
        private readonly EnumConverter _enumConverter = new(typeof(TxSource));
        public CredentialMap()
        {
            Map(c => c.Source).TypeConverter(_enumConverter);
            Map(c => c.Name);
            Map(c => c.Value);
        }
    }
}
