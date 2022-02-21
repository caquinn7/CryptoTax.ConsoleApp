using CryptoTax.ConsoleApp.Application;
using CryptoTaxV3.Domain.Coins;

namespace CryptoTax.ConsoleApp.Commands.Implementations
{
    public class CoinCommand : ICommand
    {
        private readonly ICoins _coins;

        public CoinCommand(ICoins coins)
        {
            _coins = coins;
        }

        public void Execute(CommandArgs args)
        {
            switch (args[1])
            {
                case "import":
                    Import();
                    break;
            }
        }

        private void Import()
        {
            int count = _coins.ImportAsync().GetAwaiter().GetResult();
            Output.WriteLine(count + " coins imported");
        }
    }
}
