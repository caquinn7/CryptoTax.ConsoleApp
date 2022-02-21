using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace CryptoTaxV3.Domain.Infrastructure.DataAccess
{
    public abstract class Repository
    {
        protected readonly string connStr;

        public Repository(string connectionString)
        {
            connStr = connectionString;
        }

        static Repository()
        {
            SqlMapper.AddTypeHandler(new BoolTypeHandler());
        }

        protected int Execute(string sql, object parameters = null)
        {
            using var conn = new SQLiteConnection(connStr);
            return conn.Execute(sql, parameters);
        }

        protected int ExecuteTransaction(string sql, object parameters = null)
        {
            using var conn = new SQLiteConnection(connStr);
            conn.Open();
            using var tx = conn.BeginTransaction();

            int affected = conn.Execute(sql, parameters, tx);
            tx.Commit();
            return affected;
        }

        protected IEnumerable<T> Select<T>(string sql, object parameters = null)
        {
            using var conn = new SQLiteConnection(connStr);
            return conn.Query<T>(sql, parameters);
        }

        protected T SelectSingle<T>(string sql, object parameters = null)
        {
            using var conn = new SQLiteConnection(connStr);
            return conn.QuerySingleOrDefault<T>(sql, parameters);
        }
    }
}
