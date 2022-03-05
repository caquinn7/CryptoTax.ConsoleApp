using CryptoTax.ConsoleApp.Application;
using CryptoTaxV3.Domain.AppSettings;
using CryptoTaxV3.Domain.Products;

namespace CryptoTax.ConsoleApp.Commands.Implementations
{
    public class MarketCommand : FileBasedCommand, ICommand
    {
        private readonly IMarkets _markets;

        public MarketCommand(
            IMarkets markets,
            IAppSettings appSettings)
            : base(appSettings)
        {
            _markets = markets;
        }

        public void Execute(CommandArgs args)
        {
            switch (args[1])
            {
                // markets activate -f <path to csv with markets to activate>
                case "activate":
                    Activate(args);
                    break;
            }
        }

        private void Activate(CommandArgs args)
        {
            string filePath = GetFilePath(args, AppSettingKey.MARKETS_FILE);
            int count = _markets.ActivateFromCsv(filePath);
            Output.WriteLine(count + " markets activated");
        }
    }
}
