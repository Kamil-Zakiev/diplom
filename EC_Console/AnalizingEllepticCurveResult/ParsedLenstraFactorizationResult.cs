using System;
using System.Numerics;

namespace AnalizingEllepticCurveResult
{
    /// <summary> Результаты эллиптической кривой при попытке факторизации числа </summary>
    public class ParsedLenstraFactorizationResult
    {
        /// <summary> Делитель </summary>
        public BigInteger? Divider;

        /// <summary> Факторизуемое число </summary>
        public BigInteger TargetNumber;

        /// <summary> Потрачено времени </summary>
        public TimeSpan WastedTime;

        /// <summary> Разрядность делителя факторизуемого числа </summary>
        public int DividerDigitsCount;

        /// <summary> Успех? </summary>
        public bool Success => Divider.HasValue;
    }
}