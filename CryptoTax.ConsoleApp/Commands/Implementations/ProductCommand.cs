using CryptoTax.ConsoleApp.Application;
using CryptoTaxV3.Domain.Products;

namespace CryptoTax.ConsoleApp.Commands.Implementations
{
    public class ProductCommand : ICommand
    {
        private readonly IProducts _products;

        public ProductCommand(IProducts products)
        {
            _products = products;
        }

        public void Execute(CommandArgs args)
        {
            int count = _products.ImportFromSourcesAsync().GetAwaiter().GetResult();
            Output.WriteLine(count + " products imported");
        }
    }
}
