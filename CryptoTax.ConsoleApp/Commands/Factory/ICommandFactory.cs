namespace CryptoTax.ConsoleApp.Commands.Factory
{
    public interface ICommandFactory
    {
        ICommand GetCommand(string arg);
    }
}
