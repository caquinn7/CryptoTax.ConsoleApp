using System.Threading.Tasks;

namespace CryptoTaxV3.Domain.Transactions.Importers
{
    public interface ITxImporter
    {
        Task<ImportResult> GetTransactionsAsync();
    }
}
