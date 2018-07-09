using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace EC_Console
{
    public static class Test
    {
        /// <summary> Разложение </summary>
        public static void Factorize()
        {
            BigInteger n = BigInteger.Parse("123456794568413263") * BigInteger.Parse("256783548123456833");
            var pRs = BigIntegerExtensions.Factorize<ClassicLenstra>(n).OrderBy(x => x.Key);
            var nRecover = BigInteger.One;
            foreach (var pR in pRs)
            {
                nRecover *= BigInteger.Pow(pR.Key, pR.Value);
            }
            var eq = n == nRecover;
        }

        /// <summary> Символ Лежандра </summary>
        public static void LegendreSymbol()
        {
            int a = 27;
            int p = 23;
            Console.WriteLine("a = " + a);
            Console.WriteLine("p = " + p);
            Console.WriteLine("(a/p) = {0}", BigIntegerExtensions.Jacobi(a, p)); ;
        }

        /// <summary> Рандомное число </summary>
        public static void Random()
        {
            var random = new Random();
            BigInteger n = BigInteger.Parse("123456794568413263") * BigInteger.Parse("256783548123456833");

            var listOfRandoms = new List<BigInteger>();
            for (int i = 0; i < 1000000; i++)
            {
                var bi = BigIntegerExtensions.GetNextRandom(random, n);
                listOfRandoms.Add(bi);
            }
            var count = listOfRandoms.Distinct().Count();
        }

        /// <summary> Количество точек </summary>
        public static void CountPoints()
        {
            BigInteger n = BigInteger.Parse("123456789");
            var ec = new EllepticCurve(5, -5, n);
            var count = ec.CountPoints;
        }



    }
}
