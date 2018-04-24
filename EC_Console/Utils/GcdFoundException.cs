namespace Utils
{
    using System;
    using System.Numerics;

    /// <summary> Исключение для выхода из алгоритма Ленстры </summary>
    public class GcdFoundException : Exception
    {
        public GcdFoundException(BigInteger greatestCommonDivisor, string message = null)
        {
            Message = message ?? "Найден НОД!";
            GreatestCommonDivisor = greatestCommonDivisor;
        }

        public new string Message { get; set; }
        public BigInteger GreatestCommonDivisor { get; set; }
    }
}