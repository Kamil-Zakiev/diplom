using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace EC_Console
{
    public class Lenstra
    {
        public BigInteger B1 = BigInteger.Parse("100000");

        /// <summary> 
        /// Возвращает делитель числа n. 
        /// Если делитель не найден, возвращается 1ца
        /// </summary>
        /// <param name="n">Число, у которого требуется найти делитель</param>
        public BigInteger GetDivider(BigInteger n, Random random, CancellationToken token)
        {
            var startTime = DateTime.Now;
            BigInteger g, x, y, a, b;
            do
            {
                x = BigIntegerExtensions.GetNextRandom(random, n);
                y = BigIntegerExtensions.GetNextRandom(random, n);
                a = BigIntegerExtensions.GetNextRandom(random, n);

                b = BigIntegerExtensions.Mod(y * y - x * x * x - a * x, n);
                g = BigInteger.GreatestCommonDivisor(n, 4 * a * a * a + 27 * b * b);
            } while (g == n);

            try
            {
                if (g != 1)
                    throw new GCDFoundException(g);
                //var ec = new EllepticCurve(5, -5, n);
                var ec = new EllepticCurve(a, b, n);
                var p0 = new PointOfEC()
                {
                    EllepticCurve = ec,
                    X = x,
                    Y = y
                };
                var P = new PointOfEC(p0);

                BigInteger p = 2;
                while (p < B1 && !token.IsCancellationRequested)
                {
                    var pr = p;
                    while (pr < B1)
                    {
                        P = ec.Mult(p, P);
                        pr *= p;
                    }
                    p = BigIntegerExtensions.NextPrimaryMillerRabin(p);
                }
            }
            catch (GCDFoundException exc)
            {
                //var endTime = DateTime.Now;
                //Console.WriteLine("Затрачено времени {0} секунд", (endTime - startTime).TotalSeconds.ToString("F2"));
                //Console.WriteLine("Найдено значение НОД(х2-х1, n) = {0}", exc.GreatestCommonDivisor);
                //Console.WriteLine("Разложение числа {0}", n);
                Console.WriteLine("Поток {0} молодец: {1} = {2} * {3}", 
                    Task.CurrentId, n, exc.GreatestCommonDivisor, n / exc.GreatestCommonDivisor);
                return exc.GreatestCommonDivisor;
            }


            if (token.IsCancellationRequested)
                Console.WriteLine("Поток {0} остановлен", Task.CurrentId);
            else
                Console.WriteLine("Поток {0} не смог", Task.CurrentId);
            var failRndTime = DateTime.Now;
            //Console.WriteLine("Затрачено времени {0} секунд", (failRndTime - startTime).TotalSeconds.ToString("F2"));
            //Console.WriteLine("Разложение числа {0} найти не удалось ", n);
            return BigInteger.One;
        }
    }
}