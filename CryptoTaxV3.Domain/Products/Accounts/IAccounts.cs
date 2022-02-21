using System.Collections.Generic;
using CryptoTaxV3.Domain.Products.DAL;

namespace CryptoTaxV3.Domain.Products
{
    public interface IAccounts
    {
        int ActivateFromCsv(string filePath);
        int Add(IEnumerable<AccountDto> accountDtos);
        IEnumerable<AccountDto> Get(TxSource txSource);
        IEnumerable<Account> GetActive(TxSource txSource);
    }
}