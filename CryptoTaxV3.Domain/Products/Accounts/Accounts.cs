using System.Collections.Generic;
using System.Linq;
using CryptoTaxV3.Domain.Exceptions;
using CryptoTaxV3.Domain.Infrastructure;
using CryptoTaxV3.Domain.Infrastructure.Csv;
using CryptoTaxV3.Domain.Products.DAL;
using CsvHelper.Configuration;

namespace CryptoTaxV3.Domain.Products
{
    public class Accounts : IAccounts
    {
        private readonly IAccountRepository _repo;
        private readonly ICsvReaderWrapper _csvReader;

        public Accounts(
            IAccountRepository accountRepository,
            ICsvReaderWrapper csvReader)
        {
            _repo = accountRepository;
            _csvReader = csvReader;
        }

        public IEnumerable<Account> GetActive(TxSource? source = null) =>
            _repo.GetActive(source?.ToString());

        public int Add(IEnumerable<Account> accounts)
        {
            foreach (var a in accounts)
            {
                Preconditions.ThrowValidationIfNullOrWhiteSpace(a.Asset, "Account Asset required");
                Preconditions.ThrowValidationIfNullOrWhiteSpace(a.ExternalId, "Account External Id required");
            }
            return _repo.AddOrUpdate(accounts);
        }

        public int ActivateFromCsv(string filePath)
        {
            var accounts = _csvReader.GetRecords(filePath, new AccountMap());
            var invalidAcct = accounts.FirstOrDefault(a => !_repo.Exists(a));
            if (invalidAcct != null)
            {
                string msg = $"Account not found: Source: {invalidAcct.Source}, Asset: {invalidAcct.Asset}";
                throw new ValidationException(msg);
            }
            return _repo.Activate(accounts);
        }
    }

    internal class AccountMap : ClassMap<Account>
    {
        public AccountMap()
        {
            Map(a => a.Source);
            Map(a => a.Asset);
        }
    }
}
