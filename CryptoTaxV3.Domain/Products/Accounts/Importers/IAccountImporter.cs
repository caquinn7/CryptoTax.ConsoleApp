using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoTaxV3.Domain.Products.Importers
{
    public interface IAccountImporter
    {
        Task<IEnumerable<AccountDto>> GetAccountsAsync();
    }
}