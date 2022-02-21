namespace CryptoTaxV3.Domain.Products.Importers
{
    public interface IMarketImporterFactory 
    {
        IMarketImporter GetImporter(TxSource source);
    }
}