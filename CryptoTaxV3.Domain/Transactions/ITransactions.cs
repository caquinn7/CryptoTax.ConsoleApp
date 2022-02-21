using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoTaxV3.Domain.Transactions
{
    public interface ITransactions
    {
        int Delete();
        IEnumerable<TransactionDto> Get();
        IEnumerable<TransactionDto> GetTxsWithoutUnitValue();
        Task<int> ImportFromCsvAsync(string filePath);
        Task<IEnumerable<ImportResultDto>> ImportFromSourcesAsync();
        int UpdateUnitValue(int id, decimal unitValue, long? timestamp);
        void WriteToCsv(string filePath);
    }
}