namespace LenstraAlgorithm.Dto
{
    using System;
    using System.Numerics;

    /// <summary> Результаты эллиптической кривой при попытке факторизации числа </summary>
    public class LenstraFactorizationResult
    {
        /// <summary> Делитель </summary>
        public BigInteger Divider = BigInteger.One;

        /// <summary> Эллиптическая кривая </summary>
        public IEllepticCurve EllepticCurve;

        /// <summary> Факторизуемое число </summary>
        public BigInteger TargetNumber;

        /// <summary> Потрачено времени </summary>
        public TimeSpan WastedTime;

        /// <summary> Разрядность факторизуемого числа </summary>
        public int TargetNumberDigitsCount => TargetNumber.ToString().Length;

        /// <summary> Разрядность делителя факторизуемого числа </summary>
        public int DividerDigitsCount => Divider.ToString().Length;

        /// <summary> Успех? </summary>
        public bool Success => Divider != BigInteger.One && Divider != TargetNumber;

        /// <summary> Контекст остановки факторизации </summary>
        public EEndType EndType { get; set; }

        public override string ToString()
        {
            return $"Success = {Success},  Divider = {Divider},  WastedTime= {WastedTime} ";
        }
    }
}