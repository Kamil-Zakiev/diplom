using System;

namespace Analisys
{
    using Classificators;

    /// <summary> Класс, инкапсулирующий информацию об одной эллиптической кривой </summary>
    internal class CurveInfo : CurveBaseInfo
    {
        public CurveInfo(string row)
        {
            var words = row.Split('\t');
            var i = 0;
            DigitsCountOfFiledOrder = Convert.ToInt32(words[i++]);
            ParameterA = Convert.ToInt32(words[i++]);
            ParameterB = Convert.ToInt32(words[i++]);
            FieldOrder = Convert.ToInt32(words[i++]);
            PointsCount = Convert.ToInt32(words[i++]);
            EdgeB = Convert.ToInt32(words[i]);
        }

        /// <summary> Количество разрядов в числе порядка поля </summary>
        public int DigitsCountOfFiledOrder { get; }

        /// <summary> Параметр кривой "а" </summary>
        public int ParameterA { get; }

        /// <summary> Параметр кривой "b" </summary>
        public int ParameterB { get; }
    }
}