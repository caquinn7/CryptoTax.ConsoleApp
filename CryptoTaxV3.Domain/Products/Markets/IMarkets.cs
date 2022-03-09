using System.Collections.Generic;
using CryptoTaxV3.Domain.Products.DAL;

namespace CryptoTaxV3.Domain.Products
{
    public interface IMarkets
    {
        int Activate(IEnumerable<Market> markets);
        int Add(IEnumerable<Market> markets);
        IEnumerable<Market> GetActive(TxSource source);
    }
}
