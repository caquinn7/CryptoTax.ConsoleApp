using System.Collections.Generic;
using System.Data.SQLite;
using CryptoTaxV3.Domain.Infrastructure.DataAccess;
using Dapper;

namespace CryptoTaxV3.Domain.Credentials.DAL
{
    public class CredentialRepository : Repository, ICredentialRepository
    {
        public CredentialRepository(string connectionString) : base(connectionString)
        {
        }

        public int Insert(IEnumerable<Credential> credentials)
        {
            using var conn = new SQLiteConnection(connStr);
            conn.Open();
            using var tx = conn.BeginTransaction();

            conn.Execute("delete from source_credentials", transaction: tx);
            int count = conn.Execute(@"
                insert into source_credentials (source, name, value)
                values (@Source, @Name, @Value)", credentials, tx);

            tx.Commit();
            return count;
        }

        public string GetCredentialValue(string source, string credentialName) =>
            SelectSingle<string>(@"
                select value
                from source_credentials
                where source = @source and name = @credentialName",
                new { source, credentialName });

        public IEnumerable<CredentialDto> GetCredentials() =>
            Select<CredentialDto>(@"
                select id, source, name, value
                from source_credentials
                order by source, name");
    }
}
