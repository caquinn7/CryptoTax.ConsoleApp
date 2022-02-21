using System;

namespace CryptoTaxV3.Domain.FormEntries
{
    public class FormEntry
    {
        public FormEntry(
            string asset,
            decimal quantity,
            DateTime dateAcquired,
            DateTime dateSold,
            decimal costBasis,
            decimal proceeds)
        {
            if (string.IsNullOrWhiteSpace(asset))
                throw new ArgumentException($"{nameof(asset)} is required");
            if (quantity <= 0m)
                throw new ArgumentException($"{nameof(quantity)} must be a positive number");
            if (dateAcquired == default)
                throw new ArgumentException($"{nameof(dateAcquired)} is required");
            if (dateSold == default)
                throw new ArgumentException($"{nameof(dateSold)} is required");
            if (costBasis <= 0m)
                throw new ArgumentException($"{nameof(costBasis)} must be a positive number");
            if (proceeds <= 0m)
                throw new ArgumentException($"{nameof(proceeds)} must be a positive number");

            Asset = asset;
            Quantity = quantity;
            DateAcquired = dateAcquired;
            DateSold = dateSold;
            CostBasis = decimal.Round(costBasis, 2, MidpointRounding.AwayFromZero);
            Proceeds = decimal.Round(proceeds, 2, MidpointRounding.AwayFromZero);
        }

        public string Asset { get; }
        public decimal Quantity { get; }
        public DateTime DateAcquired { get; }
        public DateTime DateSold { get; }
        public decimal CostBasis { get; }
        public decimal Proceeds { get; }
        public decimal Gain => Proceeds - CostBasis;

        public bool IsLongTerm =>
            GetCalendarYearsPassed(DateAcquired.Date.AddDays(1), DateSold.Date) >= 1;

        public static int GetCalendarYearsPassed(DateTime startDate, DateTime endDate)
        {
            bool sameMonth = startDate.Month == endDate.Month;
            bool startDayGreater = endDate.Day < startDate.Day;
            bool startMonthGreater = endDate.Month < startDate.Month;

            //Excel documentation says "COMPLETE calendar years in between dates"
            int years = endDate.Year - startDate.Year;
            if (sameMonth && startDayGreater || startMonthGreater)
            {
                years--;
            }
            return years;
        }
    }
}
