using System;

namespace CryptoTaxV3.Domain.Transactions
{
    public class ImportResultDto
    {
        public TxSource Source { get; set; }
        public int? Count { get; set; }
        public Guid? ErrorId { get; set; }
    }
}
