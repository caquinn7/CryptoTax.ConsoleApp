using System.Collections.Generic;
using System.Data.SQLite;
using CryptoTaxV3.Domain.Infrastructure.DataAccess;
using Dapper;

namespace CryptoTaxV3.Domain.Products.DAL
{
    public class AccountRepository : Repository, IAccountRepository
    {
        public AccountRepository(string connectionString) : base(connectionString)
        {
        }

        public int AddOrUpdate(IEnumerable<Account> accounts) =>
            ExecuteTransaction(@"
                insert into accounts (source, asset, external_id, is_active)
                values (@Source, @Asset, @ExternalId, @IsActive)
                on conflict (source, asset) do update
                set external_id = excluded.external_id", accounts);

        public IEnumerable<AccountDto> Get(string source) =>
            Select<AccountDto>(@"
                select 
	                id,
                    source,
                    asset,
                    external_id ExternalId,
                    is_active IsActive
                from accounts
                where source = @source
                order by asset", new { source });

        public IEnumerable<Account> GetActive(string source) =>
            Select<Account>(@"
                select 
	                id,
                    source,
                    asset,
                    external_id ExternalId,
                    is_active IsActive
                from accounts
                where source = @source and is_active = 1
                order by asset", new { source });

        public int Activate(IEnumerable<Account> accounts)
        {
            using var conn = new SQLiteConnection(connStr);
            conn.Open();
            using var tx = conn.BeginTransaction();

            conn.Execute("update accounts set is_active = 0", transaction: tx);
            int count = conn.Execute(@"
                update accounts
                set is_active = 1
                where source = @Source and asset = @Asset", accounts, tx);

            tx.Commit();
            return count;
        }

        public bool Exists(Account account) =>
            SelectSingle<bool>(@"
                select exists(
                    select 1 from accounts
                    where source = @Source and asset = @Asset
                )", account);
    }
}