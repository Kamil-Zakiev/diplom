namespace LenstraAlgorithm.Dto
{
    using System;
    using System.Numerics;

    /// <summary> Информация о времени факторизации числа </summary>
    public class FactorizeTimeResult
    {
        /// <summary> Размерность факторизуемого числа </summary>
        public int FactorizedNumberDimension => FactorizedNumber.ToString().Length;

        /// <summary> Факторизуемое число </summary>
        public BigInteger FactorizedNumber { get; set; }

        /// <summary> Размерность делителя </summary>
        /// <exception cref="InvalidOperationException">Если делитель не был найден</exception>
        public int DividerDimension
        {
            get
            {
                if (Divider != null)
                {
                    return Divider.Value.ToString().Length;
                }

                throw new InvalidOperationException();
            }
        }

        /// <summary> Делитель. Если не найден, то null </summary>
        public BigInteger? Divider { get; set; }

        /// <summary> Признак успешности факторизации числа </summary>
        public bool Success => Divider.HasValue;

        /// <summary> Затраченное время </summary>
        public TimeSpan? TimeSpan { get; set; }
    }
}