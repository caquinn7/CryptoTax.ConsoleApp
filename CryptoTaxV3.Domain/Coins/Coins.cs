using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Coins.DAL;
using CryptoTaxV3.Domain.Integrations.CoinPaprika;

namespace CryptoTaxV3.Domain.Coins
{
    public class Coins : ICoins
    {
        private readonly ICoinRepository _coinRepo;
        private readonly ICoinPaprikaClient _coinPaprikaClient;

        public Coins(
            ICoinRepository coinRepository,
            ICoinPaprikaClient coinPaprikaClient)
        {
            _coinRepo = coinRepository;
            _coinPaprikaClient = coinPaprikaClient;
        }

        public Coin Get(string id) => _coinRepo.Get(id);

        public IEnumerable<Coin> Get() => _coinRepo.Get();

        public async Task<int> ImportAsync()
        {
            var coins = await _coinPaprikaClient.GetCoinsAsync();
            foreach (Coin coin in coins)
            {
                bool requiredFieldMissing = string.IsNullOrWhiteSpace(coin.Id) || string.IsNullOrWhiteSpace(coin.Symbol);
                if (requiredFieldMissing) continue;
            }
            return _coinRepo.AddOrUpdate(coins);
        }
    }
}
