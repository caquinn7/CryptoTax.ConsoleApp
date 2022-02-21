using System.Collections.Generic;

namespace CryptoTaxV3.Domain.Coins.DAL
{
    public interface ICoinRepository
    {
        Coin Get(string id);
        IEnumerable<Coin> Get();
        int AddOrUpdate(IEnumerable<Coin> coins);
    }
}