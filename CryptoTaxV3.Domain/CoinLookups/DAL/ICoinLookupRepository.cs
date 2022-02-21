using System.Collections.Generic;
using CryptoTaxV3.Domain.Coins.DAL;

namespace CryptoTaxV3.Domain.CoinLookups.DAL
{
    public interface ICoinLookupRepository
    {
        string GetCoinId(string asset);
        CoinLookup Get(string asset);
        IEnumerable<CoinLookup> Get();
        int Insert(IEnumerable<CoinLookup> coinLookups);
    }
}