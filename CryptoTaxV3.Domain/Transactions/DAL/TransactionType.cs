namespace CryptoTaxV3.Domain.Transactions.DAL
{
    public class TransactionType
    {
        public string Name { get; set; }
        public bool IsIncoming { get; set; }
        public bool IsOutgoing { get; set; }
    }
}
