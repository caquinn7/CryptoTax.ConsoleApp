using System;
using System.Collections.Generic;
using CryptoTaxV3.Domain.Transactions.DAL;

namespace CryptoTaxV3.Domain.Transactions.Importers
{
    public class ImportResult
    {
        public IEnumerable<Transaction> Transactions { get; set; }
        public TxSource Source { get; set; }
        public Guid? ErrorId { get; set; }
    }
}
