using System;
using System.Numerics;

namespace EC_Console
{
    /// <summary> Резльтаты эллиптической кривой при попытке факторизации числа </summary>
    public class LenstraResultOfEllepticCurve
    {
        /// <summary> Факторизуемое число </summary>
        public BigInteger TargetNumber;

        /// <summary> Эллиптическая кривая </summary>
        public EllepticCurve EllepticCurve;

        /// <summary> Потрачено времени </summary>
        public TimeSpan WastedTime;

        /// <summary> Делитель </summary>
        public BigInteger Divider = BigInteger.One;

        /// <summary> Успех? </summary>
        public bool Success { get { return Divider != BigInteger.One; } }
    }
}
