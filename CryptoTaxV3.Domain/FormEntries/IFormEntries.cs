namespace CryptoTaxV3.Domain.FormEntries
{
    public interface IFormEntries
    {
        void ExportToCsv(int? taxYear, string asset, string folderPath, bool splitByTerm);
    }
}