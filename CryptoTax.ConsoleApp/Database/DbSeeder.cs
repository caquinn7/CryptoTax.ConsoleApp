using System.Data.SQLite;
using System.IO;
using System.Reflection;
using CryptoTaxV3.Domain;
using CryptoTaxV3.Domain.Transactions.DAL;

namespace CryptoTax.ConsoleApp.Database
{
    public class DbSeeder
    {
        private readonly SQLiteConnection _conn;

        public DbSeeder(SQLiteConnection connection)
        {
            _conn = connection;
            _conn.Open();
        }

        public void CreateTables()
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = GetTablesSql();
            cmd.ExecuteNonQuery();
        }
        
        public void DropTables()
        {
            var tables = new string[]
            {
                "transactions",
                "transaction_types",
                "batches",
                "coin_lookup",
                "coins",
                "accounts",
                "markets",
                "products",
                "source_credentials",
                "sources",
                "product_types",
                "app_settings",
            };
            foreach (var table in tables)
            {
                using var cmd = _conn.CreateCommand();
                cmd.CommandText = $"drop table if exists {table}";
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertSources()
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = $@"
                insert or ignore into sources (name, product_type, market_hyphenated, is_active)
                values
                    ('{TxSource.Binance}',       '{ProductType.Market}',     0,      1),
                    ('{TxSource.BinanceUS}',     '{ProductType.Market}',     0,      1),
                    ('{TxSource.Coinbase}',      '{ProductType.Account}',    null,   1),
                    ('{TxSource.CoinbasePro}',   '{ProductType.Market}',     1,      1),
                    ('{TxSource.CoinEx}',        '{ProductType.Market}',     0,      1),
                    ('{TxSource.Kraken}',        null,                       null,   1)";
            cmd.ExecuteNonQuery();
        }

        public void InsertTransactionTypes()
        {
            var txTypes = new TransactionType[]
            {
                new TransactionType { Name = TxType.Buy.ToString(),         IsIncoming = true,  IsOutgoing = false },
                new TransactionType { Name = TxType.Fork.ToString(),        IsIncoming = true,  IsOutgoing = false },
                new TransactionType { Name = TxType.Gift.ToString(),        IsIncoming = true,  IsOutgoing = true },
                new TransactionType { Name = TxType.Income.ToString(),      IsIncoming = true,  IsOutgoing = false },
                new TransactionType { Name = TxType.Lost.ToString(),        IsIncoming = false, IsOutgoing = true },
                new TransactionType { Name = TxType.Mining.ToString(),      IsIncoming = true,  IsOutgoing = false },
                new TransactionType { Name = TxType.NetworkFee.ToString(),  IsIncoming = false, IsOutgoing = true },
                new TransactionType { Name = TxType.Payment.ToString(),     IsIncoming = false, IsOutgoing = true },
                new TransactionType { Name = TxType.Sell.ToString(),        IsIncoming = false, IsOutgoing = true },
                new TransactionType { Name = TxType.Trade.ToString(),       IsIncoming = true,  IsOutgoing = true },
                new TransactionType { Name = TxType.TradingFee.ToString(),  IsIncoming = false, IsOutgoing = true },
            };

            foreach (var type in txTypes)
            {
                using var cmd = _conn.CreateCommand();
                cmd.CommandText = @"
                    insert or ignore into transaction_types (name, is_incoming, is_outgoing)
                    values (@name, @isIncoming, @isOutgoing)";
                cmd.Parameters.AddWithValue("@name", type.Name);
                cmd.Parameters.AddWithValue("@isIncoming", type.IsIncoming ? 1 : 0);
                cmd.Parameters.AddWithValue("@isOutgoing", type.IsOutgoing ? 1 : 0);
                cmd.ExecuteNonQuery();
            }
        }

        private static string GetTablesSql()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("CryptoTax.ConsoleApp.Database.db_schema.sql");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
