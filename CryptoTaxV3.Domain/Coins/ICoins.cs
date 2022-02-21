using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTaxV3.Domain.Coins.DAL;

namespace CryptoTaxV3.Domain.Coins
{
    public interface ICoins
    {
        IEnumerable<Coin> Get();
        Coin Get(string id);
        Task<int> ImportAsync();
    }
}