using CryptoTax.ConsoleApp.Application;
using CryptoTaxV3.Domain.AppSettings;

namespace CryptoTax.ConsoleApp.Commands.Implementations
{
    public class AppSettingCommand : ICommand
    {
        private readonly IAppSettings _appSettings;

        public AppSettingCommand(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public void Execute(CommandArgs args)
        {
            var results = _appSettings.Get();
            Output.WriteTable(results);
        }
    }
}
