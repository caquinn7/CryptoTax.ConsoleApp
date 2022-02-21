using System.Collections.Generic;
using System.Data.SQLite;
using CryptoTaxV3.Domain.Infrastructure.DataAccess;
using Dapper;

namespace CryptoTaxV3.Domain.CoinLookups.DAL
{
    public class CoinLookupRepository : Repository, ICoinLookupRepository
    {
        public CoinLookupRepository(string connectionString) : base(connectionString)
        {
        }

        public int Insert(IEnumerable<CoinLookup> coinLookups)
        {
            using var conn = new SQLiteConnection(connStr);
            conn.Open();
            using var tx = conn.BeginTransaction();

            conn.Execute("delete from coin_lookup", transaction: tx);
            int count = conn.Execute(@"
                insert into coin_lookup (asset, coin_id)
                values (@Asset, @CoinId)", coinLookups, tx);

            tx.Commit();
            return count;
        }

        public IEnumerable<CoinLookup> Get() =>
            Select<CoinLookup>(@"
                select asset, coin_id CoinId
                from coin_lookup
                order by coin_id");

        public CoinLookup Get(string asset) =>
            SelectSingle<CoinLookup>(@"
                select asset, coin_id CoinId
                from coin_lookup
                where asset = @asset", new { asset });

        public string GetCoinId(string asset) =>
            SelectSingle<string>(
                "select coin_id from coin_lookup where asset = @asset",
                new { asset });
    }
}