using CryptoTax.ConsoleApp.Application;
using CryptoTaxV3.Domain.Sources;

namespace CryptoTax.ConsoleApp.Commands.Implementations
{
    public class SourceCommand : ICommand
    {
        private readonly ISources _sources;

        public SourceCommand(ISources sources)
        {
            _sources = sources;
        }

        public void Execute(CommandArgs args)
        {
            if (args.Length == 1)
            {
                var sources = _sources.GetDtos();
                Output.WriteTable(sources);
                return;
            }
        }
    }
}
