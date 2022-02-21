using System.Collections.Generic;

namespace CryptoTaxV3.Domain.Products.DAL
{
    public interface IAccountRepository
    {
        int Activate(IEnumerable<Account> accounts);
        bool Exists(Account account);
        IEnumerable<AccountDto> Get(string source);
        IEnumerable<Account> GetActive(string source);
        int AddOrUpdate(IEnumerable<Account> markets);
    }
}