using System;
using System.Linq;
using CryptoTax.ConsoleApp.Application;
using CryptoTaxV3.Domain;
using CryptoTaxV3.Domain.AppSettings;
using CryptoTaxV3.Domain.Products;
using CryptoTaxV3.Domain.Sources;

namespace CryptoTax.ConsoleApp.Commands.Implementations
{
    public class MarketCommand : FileBasedCommand, ICommand
    {
        private readonly IMarkets _markets;
        private readonly ISources _sources;

        public MarketCommand(
            IMarkets markets,
            ISources sources,
            IAppSettings appSettings)
            : base(appSettings)
        {
            _markets = markets;
            _sources = sources;
        }

        public void Execute(CommandArgs args)
        {
            if (args.Length == 1)
            {
                var activeMarkets = _sources.GetActive()
                    .SelectMany(s => _markets.Get(Enum.Parse<TxSource>(s.Name, ignoreCase: true)))
                    .Where(m => m.IsActive);

                Output.WriteTable(activeMarkets);
                return;
            }

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
