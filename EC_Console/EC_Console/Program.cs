using System;
using System.Numerics;

namespace EC_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var startTime = DateTime.Now;
            //var ec = new EllepticCurve(1, 1, 23);
            //var p1 = ec.CreatePoint(0, 1);
            //var p2 = ec.Sum(p1, p1);

            //for (int i = 0; i < 100; i++)
            //{
            //    p2 = ec.Sum(p2, p1);
            //    if(!p2.Equals(ec.Mult(i+3,p1)))
            //        throw new Exception("qqweqweqwe");
            //    Console.WriteLine(p2);
            //}

            BigInteger B1 = BigInteger.Parse("10000000");
            BigInteger n = BigInteger.Parse((13*11).ToString());

            var random = new Random();
            BigInteger g, x, y, a, b;
            do
            {
                x = BigIntegerExtensions.GetNextRandom(random, n);
                y = BigIntegerExtensions.GetNextRandom(random, n);
                a = BigIntegerExtensions.GetNextRandom(random, n);

                b = BigIntegerExtensions.Mod(y * y - x * x * x - a * x, n);
                g = BigInteger.GreatestCommonDivisor(n, 4 * a * a * a + 27 * b * b);
            } while (g == n);

            if (g != 1)
                throw new Exception("Делитель найден");
            //var ec = new EllepticCurve(5, -5, n);
            var ec = new EllepticCurve(a, b, n);
            var p0 = new PointOfEC()
            {
                EllepticCurve = ec,
                X = x,
                Y = y
            };

            try
            {
                var P = new PointOfEC(p0);

                BigInteger p = 2;
                while (p < B1)
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
                var endTime = DateTime.Now;
                Console.WriteLine("Затрачено времени {0} секунд", (endTime - startTime).TotalSeconds.ToString("F2"));
                Console.WriteLine("Найдено значение НОД(х2-х1, n) = {0}", exc.GreatestCommonDivisor);
                Console.WriteLine("Разложение числа {0}", n);
                Console.WriteLine("{0}={1} * {2}", n, exc.GreatestCommonDivisor, n / exc.GreatestCommonDivisor);
            }


        }
    }
}
