using System;

namespace Analisys
{
    /// <summary>
    ///     Класс, инкапсулирующий информацию об одной эллиптической кривой
    /// </summary>
    internal class CurveInfo
    {
        private CurveClass? _constantsBasedClass;

        private CurveClass? _fieldOrderBasedClass;

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

        /// <summary> Порядок конечного поля </summary>
        public int FieldOrder { get; }

        /// <summary> Количество точек эллиптической кривой </summary>
        public int PointsCount { get; }

        /// <summary> Граница B </summary>
        public int EdgeB { get; }

        /// <summary> Класс на основе порядка конечного поля </summary>
        public CurveClass FieldOrderBasedClass
        {
            get
            {
                if (_fieldOrderBasedClass.HasValue) return _fieldOrderBasedClass.Value;

                _fieldOrderBasedClass = GetFieldOrderBasedClass();
                return _fieldOrderBasedClass.Value;
            }
        }

        /// <summary> Класс на основе констант </summary>
        public CurveClass ConstantsBasedClass
        {
            get
            {
                if (_constantsBasedClass.HasValue) return _constantsBasedClass.Value;

                _constantsBasedClass = GetConstantsBasedClass();
                return _constantsBasedClass.Value;
            }
        }

        private CurveClass GetFieldOrderBasedClass()
        {
            if (EdgeB <= Math.Pow(FieldOrder, 0.25)) return CurveClass.FirstClass;

            if (EdgeB <= Math.Pow(FieldOrder, 0.33)) return CurveClass.SecondClass;

            if (EdgeB <= Math.Pow(FieldOrder, 0.5)) return CurveClass.ThirdClass;

            return CurveClass.FourthClass;
        }

        private CurveClass GetConstantsBasedClass()
        {
            if (EdgeB <= 100) return CurveClass.FirstClass;

            if (EdgeB <= 1000) return CurveClass.SecondClass;

            if (EdgeB <= 10000) return CurveClass.ThirdClass;

            return CurveClass.FourthClass;
        }
    }
}