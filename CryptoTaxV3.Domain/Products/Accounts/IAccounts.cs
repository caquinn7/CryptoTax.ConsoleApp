using System.Collections.Generic;
using CryptoTaxV3.Domain.Products.DAL;

namespace CryptoTaxV3.Domain.Products
{
    public interface IAccounts
    {
        int Activate(IEnumerable<Account> accounts);
        int Add(IEnumerable<Account> accounts);
        IEnumerable<Account> GetActive(TxSource? source = null);
    }
}
