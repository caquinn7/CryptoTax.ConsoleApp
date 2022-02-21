namespace CryptoTaxV3.Domain.Transactions.Importers
{
    public interface ITxImporterFactory
    {
        ITxImporter GetImporter(TxSource source);
    }
}