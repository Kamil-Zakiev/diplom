using System;
using System.Numerics;

namespace Utils
{
    /// <summary>
    /// Исключение для выхода из алгоритма Ленстры
    /// </summary>
    public class GcdFoundException: Exception
    {
        public new string Message { get; set; }
        public BigInteger GreatestCommonDivisor { get; set; }
        public GcdFoundException(BigInteger greatestCommonDivisor, string message = null)
        {
            Message = message ?? "Найден НОД!";
            this.GreatestCommonDivisor = greatestCommonDivisor;
        }
    }
}
