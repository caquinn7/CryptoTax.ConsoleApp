using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Integrations.Coinbase;

namespace CryptoTaxV3.Domain.Products.Importers
{
    public class CoinbaseAccountImporter : IAccountImporter
    {
        private readonly ICoinbaseClient _coinbaseClient;

        public CoinbaseAccountImporter(ICoinbaseClient coinbaseClient)
        {
            _coinbaseClient = coinbaseClient;
        }

        public async Task<IEnumerable<AccountDto>> GetAccountsAsync()
        {
            var accountDtos = await _coinbaseClient.GetAccountsAsync();
            return accountDtos.Select(a => new AccountDto
            {
                Source = TxSource.Coinbase,
                Asset = a.Currency.Code,
                ExternalId = a.Id
            });
        }
    }
}
