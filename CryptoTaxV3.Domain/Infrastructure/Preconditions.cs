using CryptoTaxV3.Domain.Exceptions;

namespace CryptoTaxV3.Domain.Infrastructure
{
    public static class Preconditions
    {
        public static void ThrowValidationIfNullOrWhiteSpace(string arg, string msg)
        {
            if (string.IsNullOrWhiteSpace(arg))
            {
                throw new ValidationException(msg);
            }
        }

        public static void ThrowValidationIfNull(object obj, string msg)
        {
            if (obj == null)
            {
                throw new ValidationException(msg);
            }
        }
    }
}
