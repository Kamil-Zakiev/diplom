using System.Numerics;

namespace EC_Console
{
    /// <summary> Точка эллиптической кривой y^2 = x^3 + a * x + b, при характеристике поля p>=3  </summary>
    public class PointOfEC
    {
        /// <summary>
        /// Координата по оси Х. Всегда в пределах [0, EllepticCurve.p)
        /// </summary>
        private BigInteger _x;

        /// <summary>
        /// Координата по оси Y. Всегда в пределах [0, EllepticCurve.p) 
        /// </summary>
        private BigInteger _y;

        public BigInteger X
        {
            get { return _x; }
            set
            {
                if (value.Sign < 0)
                    _x = value % EllepticCurve.p + EllepticCurve.p;
                else
                {
                    _x = value % EllepticCurve.p;
                }
            }
        }

        public BigInteger Y
        {
            get { return _y; }
            set
            {
                if (value.Sign < 0)
                    _y = value % EllepticCurve.p + EllepticCurve.p;
                else
                {
                    _y = value % EllepticCurve.p;
                }
            }
        }

        /// <summary> Эллиптическая кривая, которой принадлежит эта точка </summary>
        public EllepticCurve EllepticCurve { get; set; }

        /// <summary> Является ли точка ЭК точкой "в бесконечности"  </summary>
        public bool IsInfinity;

        /// <summary>
        /// Получить обратную точку
        /// Точка + Обратная точка = Точка "в бесконечности"
        /// </summary>
        public PointOfEC Invariant()
        {
            return new PointOfEC()
            {
                EllepticCurve = this.EllepticCurve,
                X = X,
                Y = -1 * Y
            };
        }

        public PointOfEC(BigInteger x, BigInteger y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            if (IsInfinity)
                return "Бесконечно удаленная точка";
            return string.Format("({0},{1})", X, Y);
        }
        
        /// <summary> Сравнение на равенство двух точек </summary>
        public bool Equals(PointOfEC point)
        {
            return this == point || this.IsInfinity && point.IsInfinity || X == point.X && Y == point.Y;
        }

        public PointOfEC()
        {
        }

        public PointOfEC(PointOfEC p)
        {
            EllepticCurve = p.EllepticCurve;
            X = p.X;
            Y = p.Y;
            IsInfinity = p.IsInfinity;
        }
    }
}
