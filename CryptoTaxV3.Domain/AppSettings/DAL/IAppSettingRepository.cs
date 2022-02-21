namespace CryptoTaxV3.Domain.AppSettings.DAL
{
    public interface IAppSettingRepository
    {
        T Get<T>(string key);
        void AddOrUpdate<T>(string key, T value);
    }
}