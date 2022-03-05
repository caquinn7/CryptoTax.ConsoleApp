using System.Collections.Generic;
using CryptoTaxV3.Domain.Products.DAL;

namespace CryptoTaxV3.Domain.Products
{
    public interface IMarkets
    {
        int ActivateFromCsv(string filePath);
        int Add(IEnumerable<Market> markets);
        IEnumerable<Market> GetActive(TxSource? source = null);
    }
}
