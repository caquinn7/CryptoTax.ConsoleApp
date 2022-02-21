using System.Collections.Generic;
using System.Data.SQLite;
using CryptoTaxV3.Domain.Infrastructure.DataAccess;
using Dapper;

namespace CryptoTaxV3.Domain.Transactions.DAL
{
    public class TransactionRepository : Repository, ITransactionRepository
    {
        public TransactionRepository(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<Transaction> Get() =>
            Select<Transaction>(@"
                select
	                id,
	                asset,
	                quantity,
	                unit_value UnitValue,
                    unit_value_timestamp UnitValueTimestamp,
	                type,
                    timestamp,
                    source,
                    external_id ExternalId,
                    batch_id BatchId
                from transactions
                order by timestamp desc");

        public Transaction Get(int id) =>
            SelectSingle<Transaction>(@"
                select
	                id,
	                asset,
	                quantity,
	                unit_value UnitValue,
                    unit_value_timestamp UnitValueTimestamp,
	                type,
                    timestamp,
                    source,
                    external_id ExternalId,
                    batch_id BatchId
                from transactions
                where id = @id", new { id });

        public IEnumerable<TransactionDto> GetTxDtos() =>
            Select<TransactionDto>(@"
                select
	                id,
	                asset,
	                quantity,
	                unit_value UnitValue,
	                source,
	                type,
	                datetime(timestamp, 'unixepoch', 'localtime') Timestamp,
	                external_id ExternalId
                from transactions
                order by timestamp desc");

        public IEnumerable<TransactionDto> GetTxsWithoutUnitValue() =>
            Select<TransactionDto>(@"
                select
	                id,
	                asset,
	                quantity,
	                unit_value UnitValue,
	                source,
	                type,
	                datetime(timestamp, 'unixepoch', 'localtime') Timestamp,
	                external_id ExternalId
                from transactions
                where unit_value is null
                order by timestamp desc");

        public int UpdateUnitValue(int id, decimal unitvalue, long timestamp) =>
            Execute(@"
                update transactions
                set unit_value = @unitvalue, unit_value_timestamp = @timestamp
                where id = @id", new { id, unitvalue, timestamp });

        public (int batchId, int count) InsertBatch(ImportType importType, IEnumerable<Transaction> transactions)
        {
            using var conn = new SQLiteConnection(connStr);
            conn.Open();
            using var tx = conn.BeginTransaction();

            conn.Execute(
                "insert into batches (type) values (@batchType)",
                new { batchType = importType.ToString() },
                tx);

            int batchId = (int)conn.LastInsertRowId;

            int count = conn.Execute($@"
                insert into transactions (
                    asset, quantity, unit_value, unit_value_timestamp, type, timestamp, source, external_id, batch_id
                ) values (
                    @Asset, @Quantity, @UnitValue, @UnitValueTimestamp, @Type, @Timestamp, @Source, @ExternalId, {batchId}
                )", transactions, tx);

            tx.Commit();
            return (batchId, count);
        }

        public int Delete() => Execute("delete from transactions");
    }
}
