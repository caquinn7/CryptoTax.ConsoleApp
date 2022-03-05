using System.Collections.Generic;
using System.Data.SQLite;
using CryptoTaxV3.Domain.Infrastructure.DataAccess;
using Dapper;

namespace CryptoTaxV3.Domain.Products.DAL
{
    public class MarketRepository : Repository, IMarketRepository
    {
        public MarketRepository(string connectionString) : base(connectionString)
        {
        }

        public int Insert(IEnumerable<Market> markets) =>
            ExecuteTransaction(@"
                insert or ignore into markets (source, base, quote, is_active)
                values (@Source, @Base, @Quote, @IsActive)", markets);

        public IEnumerable<Market> GetActive(string source = null) =>
            Select<Market>(@"
                select 
	                id,
                    source,
	                base,
	                quote,
	                is_active IsActive
                from markets
                where
                    (@source is null or source = @source)
                    and is_active = 1
                order by base, quote", new { source });

        public int Activate(IEnumerable<Market> markets)
        {
            using var conn = new SQLiteConnection(connStr);
            conn.Open();
            using var tx = conn.BeginTransaction();

            conn.Execute("update markets set is_active = 0", transaction: tx);
            int count = conn.Execute(@"
                update markets
                set is_active = 1
                where source = @Source and base = @Base and quote = @Quote", markets, tx);

            tx.Commit();
            return count;
        }

        public bool Exists(Market market) =>
            SelectSingle<bool>(@"
                select exists(
                    select 1 from markets
                    where source = @Source and base = @Base and quote = @Quote
                )", market);
    }
}
