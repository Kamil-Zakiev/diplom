using System;
using System.Numerics;

namespace EC_Console
{
    /// <summary>
    /// Исключение для выхода из алгоритма Ленстры
    /// </summary>
    public class GCDFoundException: Exception
    {
        public new string Message { get; set; }
        public BigInteger GreatestCommonDivisor { get; set; }
        public GCDFoundException(BigInteger greatestCommonDivisor)
        {
            Message = "Найден НОД!";
            this.GreatestCommonDivisor = greatestCommonDivisor;
        }
    }
}
