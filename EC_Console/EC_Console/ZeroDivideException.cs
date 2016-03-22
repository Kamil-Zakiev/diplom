using System;
using System.Numerics;

namespace EC_Console
{
    /// <summary>
    /// Исклю
    /// </summary>
    public class GCDFoundException: Exception
    {
        public new string Message { get; set; }
        public BigInteger GreatestCommonDivisor { get; set; }
        public GCDFoundException(BigInteger greatestCommonDivisor)
        {
            Message = "Деление на ноль!";
            this.GreatestCommonDivisor = greatestCommonDivisor;
        }
    }
}
