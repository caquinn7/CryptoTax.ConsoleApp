using System.Collections.Generic;
using CryptoTaxV3.Domain.CoinLookups.DAL;

namespace CryptoTaxV3.Domain.CoinLookups
{
    public interface ICoinLookups
    {
        IEnumerable<CoinLookup> Get();
        CoinLookup Get(string symbol);
        string GetCoinId(string asset);
        int ImportFromCsv(string filePath);
    }
}