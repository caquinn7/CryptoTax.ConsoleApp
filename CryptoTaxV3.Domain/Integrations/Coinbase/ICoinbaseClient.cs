using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoTaxV3.Domain.Integrations.Coinbase
{
    public interface ICoinbaseClient
    {
        Task<IEnumerable<CoinbaseAccountDto>> GetAccountsAsync();
        Task<IEnumerable<CoinbaseTransactionDto>> GetTransactionsAsync(string accountId);
    }
}