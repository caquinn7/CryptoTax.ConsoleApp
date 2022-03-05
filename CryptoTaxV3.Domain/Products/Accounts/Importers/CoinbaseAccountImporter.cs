using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Integrations.Coinbase;
using CryptoTaxV3.Domain.Products.DAL;

namespace CryptoTaxV3.Domain.Products.Importers
{
    public class CoinbaseAccountImporter : IAccountImporter
    {
        private readonly ICoinbaseClient _coinbaseClient;

        public CoinbaseAccountImporter(ICoinbaseClient coinbaseClient)
        {
            _coinbaseClient = coinbaseClient;
        }

        public async Task<IEnumerable<Account>> GetAccountsAsync()
        {
            var accountDtos = await _coinbaseClient.GetAccountsAsync();
            return accountDtos.Select(a => new Account
            {
                Source = TxSource.Coinbase.FastToString(),
                Asset = a.Currency.Code,
                ExternalId = a.Id
            });
        }
    }
}
