using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace EC_Console
{
    /// <summary> 
    ///Эллиптическая кривая y^2 = x^3 + a * x + b над полем F_q, при характеристике поля p>=3 
    ///</summary>
    public class EllepticCurve
    {
        /// <summary> Генератор </summary>
        public PointOfEC Basis;

        /// <summary> a принадлежит множеству F_q</summary>
        private BigInteger a;

        /// <summary> b принадлежит множеству F_q</summary>
        private BigInteger b;

        /// <summary> 
        /// Характеристика конечного поля F_q 
        /// ВНИМАНИЕ: p >= 3
        ///</summary>
        public BigInteger p;

        /// <summary> конечное поле F_q характеристики p, где q = p ^ k </summary>
        private BigInteger k;

        /// <summary> 
        ///Дискриминант эллиптической кривой 
        /// Если != 0, то ЭК неособая, то есть у нее нет кратных корней 
        ///</summary>
        public BigInteger Delta
        {
            get
            {
                return -16 * (4 * a * a * a + 27 * b * b);
            }
        }

        /// <summary>
        /// y^2 = x^3 + a * x + b 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="p"></param>
        public EllepticCurve(BigInteger a, BigInteger b, BigInteger p)
        {
            this.a = a;
            this.b = b;
            //TODO: проверять ли на простоту число р?
            this.p = p;

            //this.Basis = CreateBasis();
        }

        private long tenExp = 1000000000000000000;

        public void CreateBasis()
        {
            BigInteger x = 0;
            var y = Math.Sqrt((double)BigIntegerExtensions.Mod(x * x * x + a * x + b, p));
            while (x < p && ((long)(y * tenExp) % tenExp != 0))
            {
                x++;
                y = Math.Sqrt((double)BigIntegerExtensions.Mod(x * x * x + a * x + b, p));
            }
            Basis = CreatePoint(x, (BigInteger)y);
        }

        /// <summary> Содержит ли ЭК точку? </summary>
        /// <param name="X">координата Х</param>
        /// <param name="Y">координата Y</param>
        /// <returns></returns>
        public bool ContainsPoint(BigInteger X, BigInteger Y)
        {
            return BigIntegerExtensions.Mod(Y * Y, p) == BigIntegerExtensions.Mod(X * X * X + a * X + b, p);
        }

        public PointOfEC CreatePoint(BigInteger X, BigInteger Y)
        {
            //проверим принадлежность точки (x,y) эллиптической кривой 
            if (!ContainsPoint(X, Y))
                throw new Exception(
                    string.Format("Точка ({0},{1}) не принадлежит кривой y^2 = x^3 + {2} * x + {3}",
                        X, Y, a, b));

            return new PointOfEC()
            {
                EllepticCurve = this,
                X = X,
                Y = Y,
                IsInfinity = false
            };
        }

        /// <summary> Сумма двух точек эллиптической кривой </summary>
        /// <param name="point1"> Первая точка</param>
        /// <param name="point2"> Вторая точка</param>
        public PointOfEC Sum(PointOfEC point1, PointOfEC point2)
        {
            if (point1.Equals(point2.Invariant()))
                return new PointOfEC()
                {
                    EllepticCurve = this,
                    IsInfinity = true
                };

            if (point1.IsInfinity)
                return point2;
            if (point2.IsInfinity)
                return point1;

            var lamdZnam = point2.X - point1.X >= BigInteger.Zero ? point2.X - point1.X : point2.X - point1.X + p;
            if (lamdZnam != 0 && BigInteger.GreatestCommonDivisor(lamdZnam, p) > 1)
                throw new GCDFoundException(BigInteger.GreatestCommonDivisor(lamdZnam, p));

            point1.X = point1.X % p;
            point2.X = point2.X % p;
            point1.Y = point1.Y % p;
            point2.Y = point2.Y % p;

            BigInteger lambda;
            if (!point1.Equals(point2))
                lambda = BigInteger.Multiply(point2.Y - point1.Y, BigIntegerExtensions.Inverse(point2.X - point1.X, p));
            else
                lambda = (3 * point1.X * point1.X + a) * BigIntegerExtensions.Inverse(2 * point2.Y, p);
            lambda = lambda % p;

            var x = lambda * lambda - point1.X - point2.X;
            var y = lambda * (point1.X - x) - point1.Y;

            var result = new PointOfEC()
            {
                EllepticCurve = this,
                X = x % p,
                Y = y % p
            };

            return new PointOfEC()
            {
                //указать явно
                //IsInfinity = false,
                EllepticCurve = this,
                X = result.X >= BigInteger.Zero ? result.X : result.X + p,
                Y = result.Y >= BigInteger.Zero ? result.Y : result.Y + p
            };
        }

        public PointOfEC Mult(BigInteger k, PointOfEC p)
        {
            if (k.IsZero)
                throw new Exception("Попытка умножить на 0");

            PointOfEC b = p;
            PointOfEC q = new PointOfEC()
            {
                IsInfinity = true
            };

            while (!k.IsZero)
            {
                if (k % 2 == 1)
                    q = Sum(q, b);
                b = Sum(b, b);
                k /= 2;
            }
            return q;
        }

        private BigInteger _pointsCount;

        /// <summary> 
        /// Количество точек ЭК
        /// символ лежандра
        /// </summary> 
        public BigInteger CountPoints
        {
            get
            {
                if (_pointsCount != BigInteger.Zero)
                    return _pointsCount;

                var pLong = (long)p;
                var tasks = new Task<int>[Environment.ProcessorCount];
                var step = pLong / (Environment.ProcessorCount-1);
                for (int i = 0; i < tasks.Length; i++)
                {
                    long start = i * step;
                    long end = start + step;
                    tasks[i] = Task.Factory.StartNew(() => countPointInner(start, end));
                }
                Task.WaitAll(tasks);

                _pointsCount = tasks.Sum(x => x.Result) + 1;
                return _pointsCount;
            }
        }

        private int countPointInner(long start, long end)
        {
            var count = 0;
            var pLong = (long)p;

            if (end >= pLong)
                end = pLong;
            for (long x = start; x < end; x++)
            {
                var z = (long)BigIntegerExtensions.Mod(x * x * x + a * x + b, p);
                if (z == 0)
                    count++;
                else
                {
                    var legendreSymbol = BigIntegerExtensions.jacobi(z, pLong);
                    if (legendreSymbol == 1)
                        count += 2;
                }
            }

            return count;
        }

        public PointOfEC LenstraStartingPoint { get; set; }

        private LenstraEdges _lenstraEdges;

        /// <summary> Границы метода Ленстры </summary>
        public LenstraEdges LenstraEdges
        {
            get
            {
                if (_lenstraEdges != null)
                    return _lenstraEdges;

                var pointsCount = this.CountPoints;
                //разложение числа pointsCount
                var pRs = BigIntegerExtensions.Factorize(pointsCount).OrderBy(x => x.Key);
                if (pRs.Count() == 1)
                {
                    _lenstraEdges = new LenstraEdges()
                    {
                        B1 = BigInteger.Pow(pRs.First().Key, pRs.First().Value),
                        B2 = 1
                    };
                    return _lenstraEdges;
                }
                var b1 = pRs.Take(pRs.Count() - 1).Max(x => BigInteger.Pow(x.Key, x.Value));
                var lastPR = pRs.Skip(pRs.Count() - 1).First();
                BigInteger b2 = 1;
                if (lastPR.Value == 1 && lastPR.Key > b1)
                    b2 = lastPR.Key;

                _lenstraEdges = new LenstraEdges()
                {
                    B1 = b1,
                    B2 = b2
                };
                return _lenstraEdges;
            }
        }

    }
}
