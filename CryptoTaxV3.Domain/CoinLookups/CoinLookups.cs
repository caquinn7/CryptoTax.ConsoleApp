using System.Collections.Generic;
using CryptoTaxV3.Domain.CoinLookups.DAL;
using CryptoTaxV3.Domain.Coins;
using CryptoTaxV3.Domain.Coins.DAL;
using CryptoTaxV3.Domain.Infrastructure;
using CryptoTaxV3.Domain.Infrastructure.Csv;

namespace CryptoTaxV3.Domain.CoinLookups
{
    public class CoinLookups : ICoinLookups
    {
        private readonly ICoinLookupRepository _repo;
        private readonly ICsvReaderWrapper _csvReader;
        private readonly ICoins _coins;

        public CoinLookups(
            ICoinLookupRepository coinLookupRepository,
            ICsvReaderWrapper csvReader,
            ICoins coins)
        {
            _repo = coinLookupRepository;
            _csvReader = csvReader;
            _coins = coins;
        }

        public int ImportFromCsv(string filePath)
        {
            var lookups = _csvReader.GetRecords<CoinLookup>(filePath);
            foreach (var lookup in lookups)
            {
                Preconditions.ThrowValidationIfNullOrWhiteSpace(lookup.Asset, "CoinLookup Asset required");
                Preconditions.ThrowValidationIfNullOrWhiteSpace(lookup.CoinId, "CoinLookup Coin Id required");

                Coin coin = _coins.Get(lookup.CoinId);
                Preconditions.ThrowValidationIfNull(coin, $@"CoinLookup CoinId ""{lookup.CoinId}"" not found.");
            }
            return _repo.Insert(lookups);
        }

        public CoinLookup Get(string symbol) => _repo.Get(symbol);

        public IEnumerable<CoinLookup> Get() => _repo.Get();

        public string GetCoinId(string asset) => _repo.GetCoinId(asset);
    }
}
