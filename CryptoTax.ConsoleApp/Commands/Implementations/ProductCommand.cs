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
            if (args.Length == 1)
            {
                ShowActiveProducts();
                return;
            }

            switch (args[1])
            {
                case "import":
                    ImportProducts();
                    break;
            }
        }

        private void ShowActiveProducts()
        {
            var products = _products.GetActive();
            Output.WriteTable(products);
        }

        private void ImportProducts()
        {
            int count = _products.ImportFromSourcesAsync().GetAwaiter().GetResult();
            Output.WriteLine(count + " products imported");
        }
    }
}
