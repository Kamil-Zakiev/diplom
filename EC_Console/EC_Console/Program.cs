using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace EC_Console
{
    class Program
    {
        public class DimensionResults
        {
            public int Dimension;
            public List<EllepticCurve> FirstClass;
            public List<EllepticCurve> SecondClass;
            public List<EllepticCurve> ThirdClass;
            public List<EllepticCurve> ForthClass;
        }

        static void Main(string[] args)
        {
            #region Опыты
            /*
            //генерирование
            for (int i = 10; i <= 10; i = i + 5)
            {
                var path = string.Format("TwoPrimesMultiple/{0}digitsNumbers.txt", i);
                TwoPrimesMultipleGenerator.GenerateTwoPrimesMultipleNumbersInFile(path, i/2);
            }

            //факторизация
            var results = new List<LenstraResultOfEllepticCurve>();
            foreach (var path in Directory.GetFiles("TwoPrimesMultiple"))
            {
                results.AddRange(MultithreadLenstra.UseThreadsParallelism(path, threadsCount: 1));
            }

            //обработка результатов
            var groupResult = results
                .Where(x => x.Success)
                .GroupBy(x => x.DividerDigitsCount,
                (key, resultsOfGroup) =>
                {
                    var r = resultsOfGroup.ToList();
                    var averageSecondsOfSuccess = r.Average(z => z.WastedTime.TotalSeconds);
                    var maxSeconds = r.Max(z => z.WastedTime.TotalSeconds);
                    var minSeconds = r.Min(z => z.WastedTime.TotalSeconds);
                    var hist = new int[10];
                    r.Select(x => x.WastedTime.TotalSeconds).OrderBy(x => x).ToList()
                        .ForEach(time =>
                        {
                            var i = (int) (time/maxSeconds*9);
                            hist[i]++;
                        });
                    return new
                    {
                        dividerDigits = key,
                        hist = hist,
                        averageSecondsOfSuccess = averageSecondsOfSuccess,
                        maxSeconds = maxSeconds,
                        minSeconds = minSeconds
                    };
                }).ToList();

            //сохранение результатов
            var strList = groupResult.Select(x => x.dividerDigits + "\t" + string.Join(",", x.hist)+ "\t" + x.averageSecondsOfSuccess + "\t" + x.maxSeconds);
            File.WriteAllLines("Result.txt", strList, Encoding.UTF8);
             */

            #endregion

            #region Разложение
            //BigInteger n = BigInteger.Parse("12345678901234567890");
            //var pRs = BigIntegerExtensions.Factorize(n).OrderBy(x => x.Key);
            //var nRecover = BigInteger.One;
            //foreach (var pR in pRs)
            //{
            //    nRecover *= BigInteger.Pow(pR.Key, pR.Value);
            //}
            //var eq = n == nRecover; 
            #endregion

            #region Символ Лежандра
            /*
            int a = 27;
            int p = 23;
            Console.WriteLine("a = " + a);
            Console.WriteLine("p = " + p);
            Console.WriteLine("(a/p) = {0}", BigIntegerExtensions.Jacobi(a, p));
            */

            #endregion

            //var pointsCount = 1008;
            ////разложение числа pointsCount
            //var pRs = BigIntegerExtensions.Factorize(pointsCount).OrderBy(x => x.Key);
            //var b21 = pRs.Take(pRs.Count() - 1).Max(x => BigInteger.Pow(x.Key, x.Value));
            //var lastPR = pRs.Skip(pRs.Count() - 1).First();
            //var b2 = b21;
            //if (lastPR.Value == 1 && lastPR.Key > b21)
            //    b2 = lastPR.Key;

            //var _lenstraEdges = new LenstraEdges()
            //{
            //    B1 = b21,
            //    B2 = b2
            //};

            List<EllepticCurve> listOfEC = new List<EllepticCurve>();
            int curvesCountPerDim = 100;
            var random = new Random();
            for (int pDim = 3; pDim < 15; pDim++)
            {
                var dim = BigInteger.Pow(10, pDim);
                for (int i = 0; i < curvesCountPerDim; i++)
                {
                    dim = BigIntegerExtensions.NextPrimaryMillerRabin(dim);
                    var a = BigIntegerExtensions.GetNextRandom(random, dim, 10);
                    var b = BigIntegerExtensions.GetNextRandom(random, dim, 10);
                    var ec = new EllepticCurve(a, b, dim);
                    listOfEC.Add(ec);
                }
            }

            var result = listOfEC.GroupBy(x => x.p.ToString().Length,
                (dim, ecSubEnum) =>
                {
                    var ecSubList = ecSubEnum.ToList();
                    return new DimensionResults()
                    {
                        Dimension = dim,
                        FirstClass = ecSubList.Where(ec =>
                        {
                            var b1 = (double)ec.LenstraEdges.B1;
                            return b1 <= Math.Pow((double)ec.p, 0.25);
                        }).ToList(),
                        SecondClass = ecSubList.Where(ec =>
                        {
                            var b1 = (double)ec.LenstraEdges.B1;
                            return Math.Pow((double)ec.p, 0.25) < b1 && b1 <= Math.Pow((double)ec.p, 0.33);
                        }).ToList(),
                        ThirdClass = ecSubList.Where(ec =>
                        {
                            var b1 = (double)ec.LenstraEdges.B1;
                            return Math.Pow((double)ec.p, 0.33) < b1 && b1 <= Math.Pow((double)ec.p, 0.5);
                        }).ToList(),
                        ForthClass = ecSubList.Where(ec =>
                        {
                            var b1 = (double)ec.LenstraEdges.B1;
                            return b1 > Math.Pow((double)ec.p, 0.5);
                        }).ToList()
                    };
                }).ToList();

            File.WriteAllLines("Ishm.txt", result.Select(x => x.Dimension + " " + x.FirstClass.Count + " " + x.SecondClass.Count + " " + x.ThirdClass.Count + " " + x.ForthClass.Count));

            Console.ReadKey();
        }
    }
}
