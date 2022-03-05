using System.Collections.Generic;

namespace CryptoTaxV3.Domain.Products.DAL
{
    public interface IAccountRepository
    {
        int Activate(IEnumerable<Account> accounts);
        bool Exists(Account account);
        IEnumerable<Account> GetActive(string source = null);
        int AddOrUpdate(IEnumerable<Account> markets);
    }
}
