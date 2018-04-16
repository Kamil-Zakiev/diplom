using System;
using System.Numerics;

namespace LenstraAlgorithm
{
    /// <summary> Резльтаты эллиптической кривой при попытке факторизации числа </summary>
    public class LenstraResultOfEllepticCurve
    {
        /// <summary> Факторизуемое число </summary>
        public BigInteger TargetNumber;

        /// <summary> Разрядность факторизуемого числа </summary>
        public int TargetNumberDigitsCount { get { return TargetNumber.ToString().Length; } }

        /// <summary> Эллиптическая кривая </summary>
        public IEllepticCurve EllepticCurve;

        /// <summary> Потрачено времени </summary>
        public TimeSpan WastedTime;

        /// <summary> Делитель </summary>
        public BigInteger Divider = BigInteger.One;

        /// <summary> Разрядность делителя факторизуемого числа </summary>
        public int DividerDigitsCount { get { return Divider.ToString().Length; } }

        /// <summary> Успех? </summary>
        public bool Success { get { return Divider != BigInteger.One; } }

        public override string ToString()
        {
            return string.Format("Success = {0},  Divider = {1},  WastedTime= {2} ", Success, Divider, WastedTime);
        }
    }
}
