namespace CryptoTaxV3.Domain.AppSettings
{
    public interface IAppSettings
    {
        void AddOrUpdate<T>(string key, T value);
        T Get<T>(string key);
    }
}