using CryptoTax.ConsoleApp.Application;

namespace CryptoTax.ConsoleApp.Commands
{
    public interface ICommand
    {
        void Execute(CommandArgs args);
    }
}
