using System.Collections.Immutable;
using System.Linq;

namespace CryptoTax.ConsoleApp.Application
{
    public class CommandArgs
    {
        private readonly ImmutableArray<string> _args;

        public CommandArgs(string text)
        {
            _args = ImmutableArray.Create(text.Split(' '));
        }

        public static CommandArgs From(string text) => new(text);

        public string this[int index] => _args[index];
        public int Length => _args.Length;
        public string Text => string.Join(' ', _args);

        public string GetArg(string key, bool isPair = true) =>
            _args.SkipWhile(a => a != key).Skip(isPair ? 1 : 0).FirstOrDefault();
    }
}
