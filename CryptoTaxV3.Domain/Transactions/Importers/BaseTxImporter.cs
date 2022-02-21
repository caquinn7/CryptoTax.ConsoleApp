using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace CryptoTaxV3.Domain.Transactions.Importers
{
    public abstract class BaseTxImporter
    {
        protected readonly ILogger<BaseTxImporter> logger;

        public BaseTxImporter(ILogger<BaseTxImporter> logger)
        {
            this.logger = logger;
        }

        protected Guid LogError(Exception ex, TxSource source)
        {
            var errId = Guid.NewGuid();
            using (logger.BeginScope(new Dictionary<string, object> { ["ErrId"] = errId }))
            {
                logger.LogError(ex, "Error importing from {source}. ErrId: {errId}", source, errId);
            };
            return errId;
        }
    }
}
