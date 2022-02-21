using System.Collections.Generic;
using CryptoTaxV3.Domain.Infrastructure.DataAccess;

namespace CryptoTaxV3.Domain.Coins.DAL
{
    public class CoinRepository : Repository, ICoinRepository
    {
        public CoinRepository(string connectionString) : base(connectionString)
        {
        }

        public int AddOrUpdate(IEnumerable<Coin> coins) =>
            ExecuteTransaction(@"
                insert into coins (id, name, symbol)
                values (@Id, @Name, @Symbol)
                on conflict(id) do update
                set name = excluded.name, symbol = excluded.symbol
                where id = excluded.id", coins);

        public IEnumerable<Coin> Get() =>
            Select<Coin>(@"
                select id, name, symbol
                from coins
                order by symbol");

        public Coin Get(string id) =>
            SelectSingle<Coin>(@"
                select id, name, symbol
                from coins
                where id = @id", new { id });
    }
}
