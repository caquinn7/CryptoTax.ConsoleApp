using CryptoTaxV3.Domain.Infrastructure.DataAccess;

namespace CryptoTaxV3.Domain.AppSettings.DAL
{
    public class AppSettingRepository : Repository, IAppSettingRepository
    {
        public AppSettingRepository(string connectionStr) : base(connectionStr)
        {
        }

        public void AddOrUpdate<T>(string key, T value) =>
            Execute(@"
                insert into app_settings (key, value) values (@key, @value)
                on conflict(key) do update
                set value = excluded.value
                where key = excluded.key", new { key, value });

        public T Get<T>(string key) =>
            SelectSingle<T>(
                "select value from app_settings where key = @key",
                new { key });
    }
}
