using System;
using System.Collections.Generic;
using System.Linq;
using CryptoTaxV3.Domain.Infrastructure.Exceptions;
using CryptoTaxV3.Domain.Transactions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CryptoTaxV3.Domain.FormEntries
{
    public static class FormEntriesGenerator
    {
        public static IEnumerable<FormEntry> GetFormEntries(
            IEnumerable<TransactionDto> txs,
            ILogger<FormEntries> logger)
        {
            IEnumerable<FormEntryTx> formEntryTxs = TransformTxs(txs);
            var outgoingTxs = formEntryTxs.Where(t => t.Side == TxSide.Out).ToList();
            var incomingTxs = formEntryTxs.Where(t => t.Side == TxSide.In).ToList();
            var result = outgoingTxs.SelectMany(ot => {
                var prevIncomingTxs = incomingTxs
                    .Where(it => it.Timestamp < ot.Timestamp)
                    .Where(it => it.Quantity != 0)
                    .ToList();
                return ProcessOutgoingTx(ot, prevIncomingTxs, logger);
            });
            return result;
        }

        private static List<FormEntry> ProcessOutgoingTx(
            FormEntryTx outgoingTx,
            List<FormEntryTx> prevIncomingTxs,
            ILogger<FormEntries> logger)
        {
            var results = new List<FormEntry>();
            for (int i = 0; i < prevIncomingTxs.Count && outgoingTx.Quantity > 0; i++)
            {
                var incomingTx = prevIncomingTxs[i];

                logger.LogDebug("outgoingTx: {tx}", outgoingTx);
                logger.LogDebug("incomingTx: {tx}", incomingTx);

                var (entryQty, inQty, outQty) = GetNewQuantities(incomingTx, outgoingTx);
                incomingTx.Quantity = inQty;
                outgoingTx.Quantity = outQty;

                if (outgoingTx.IsTaxable)
                {
                    results.Add(new FormEntry(
                        outgoingTx.Asset,
                        entryQty,
                        dateAcquired: incomingTx.Timestamp,
                        dateSold: outgoingTx.Timestamp,
                        costBasis: entryQty * incomingTx.UnitValue,
                        proceeds: entryQty * outgoingTx.UnitValue));
                }

                if (i == prevIncomingTxs.Count - 1 && outgoingTx.Quantity != 0)
                {
                    throw new InsufficientBalanceException
                    {
                        Asset = outgoingTx.Asset,
                        MissingQuantity = outgoingTx.Quantity
                    };
                }
            }
            return results;
        }

        private static IEnumerable<FormEntryTx> TransformTxs(IEnumerable<TransactionDto> txs)
        {
            bool assetsDiffer = txs.Skip(1).Any(t => t.Asset != txs.First().Asset);
            if (assetsDiffer)
            {
                throw new InvalidOperationException($"All transactions must have the same {nameof(TransactionDto.Asset)}");
            }
            return txs
                .Select(t => new FormEntryTx(t))
                .OrderBy(t => t.Timestamp);
        }

        private static (decimal entryQty, decimal inTxQty, decimal outTxQty) GetNewQuantities(FormEntryTx inTx, FormEntryTx outTx)
        {
            if (inTx.Quantity < outTx.Quantity)
            {
                return (inTx.Quantity, 0, outTx.Quantity - inTx.Quantity);
            }
            else if (inTx.Quantity > outTx.Quantity)
            {
                return (outTx.Quantity, inTx.Quantity - outTx.Quantity, 0);
            }
            return (outTx.Quantity, 0, 0);
        }

        private class FormEntryTx
        {
            public FormEntryTx(TransactionDto tx)
            {
                if (tx.Id == 0)
                    throw new ArgumentException($"{nameof(tx.Id)} is required");
                if (string.IsNullOrWhiteSpace(tx.Asset))
                    throw new ArgumentException($"{nameof(tx.Asset)} is required");
                if (tx.Quantity == 0m)
                    throw new ArgumentException($"{nameof(tx.Quantity)} cannot be zero");
                if ((tx.UnitValue ?? 0m) <= 0m)
                    throw new ArgumentException($"{nameof(tx.UnitValue)} must be a positive number");
                if (tx.Timestamp == default)
                    throw new ArgumentException($"{nameof(tx.Timestamp)} is required");

                Id = tx.Id;
                Asset = tx.Asset;
                Quantity = Math.Abs(tx.Quantity);
                UnitValue = tx.UnitValue.Value;
                Timestamp = tx.Timestamp;
                Side = tx.Quantity > 0 ? TxSide.In : TxSide.Out;
                Type = tx.Type;
            }

            public int Id { get; }
            public string Asset { get; }
            public decimal Quantity { get; set; }
            public decimal UnitValue { get; }
            public DateTime Timestamp { get; }
            public TxSide Side { get; }
            public TxType Type { get; }

            public bool IsTaxable =>
                Side == TxSide.Out
                && Type != TxType.Lost
                && Type != TxType.Gift;

            public override string ToString() => JsonConvert.SerializeObject(new
            {
                Id,
                Asset,
                Quantity,
                Side = Side.ToString(),
                Type = Type.ToString(),
                Timestamp = Timestamp.ToString("yyyy-MM-dd hh:mm:ss tt"),
                IsTaxable
            }, Formatting.None);
        }

        private enum TxSide { In, Out }
    }
}
