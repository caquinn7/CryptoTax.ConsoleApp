using System.Data;
using Dapper;

namespace CryptoTaxV3.Domain.Infrastructure.DataAccess
{
    public class BoolTypeHandler : SqlMapper.TypeHandler<bool>
    {
        public override bool Parse(object value) => value.ToString() switch
        {
            "1" => true,
            "0" => false,
            _ => throw new DataException($"{nameof(value)} must be either 0 or 1")
        };

        public override void SetValue(IDbDataParameter parameter, bool value)
        {
            parameter.Value = value.ToString();
        }
    }
}
