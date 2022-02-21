using System.Collections.Generic;

namespace CryptoTaxV3.Domain.Transactions.DAL
{
    public interface ITransactionRepository
    {
        int Delete();
        IEnumerable<Transaction> Get();
        Transaction Get(int id);
        IEnumerable<TransactionDto> GetTxDtos();
        IEnumerable<TransactionDto> GetTxsWithoutUnitValue();
        (int batchId, int count) InsertBatch(ImportType importType, IEnumerable<Transaction> transactions);
        int UpdateUnitValue(int id, decimal unitvalue, long timestamp);
    }
}