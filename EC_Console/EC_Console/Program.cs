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
            #region Практическая часть диплома. Опыты
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


            #region Теоретическая часть диплома. Оценка классов в зависимости от размерности делителя

            var random = new Random();
            for (int pDim = 2; pDim < 10; pDim++)
            {
                for (int i123 = 0; i123 < 100; i123++)
                {
                    List<EllepticCurve> listOfEC = new List<EllepticCurve>();
                    const int curvesCountPerDim = 1;
                    var dimen = BigInteger.Pow(10, pDim);
                    for (int i = 0; i < curvesCountPerDim; i++)
                    {
                        dimen = BigIntegerExtensions.NextPrimaryMillerRabin(dimen);
                        var a = BigIntegerExtensions.GetNextRandom(random, dimen);
                        var b = BigIntegerExtensions.GetNextRandom(random, dimen);
                        var ec = new EllepticCurve(a, b, dimen);
                        listOfEC.Add(ec);
                        File.AppendAllText("pointsCount_in_pdim_" + (pDim + 1) + ".txt", ec.CountPoints.ToString() + "\t\r\n");
                    }

                    listOfEC.GroupBy(x => x.p.ToString().Length,
                        (dim, ecSubEnum) =>
                        {
                            var ecSubList = ecSubEnum.ToList();
                            var r = new DimensionResults()
                            {
                                Dimension = dim,
                                FirstClass = ecSubList.Where(ec =>
                                {
                                    var b1 = (double) ec.LenstraEdges.B1;
                                    return b1 <= Math.Pow((double) ec.p, 0.25);
                                }).ToList(),
                                SecondClass = ecSubList.Where(ec =>
                                {
                                    var b1 = (double) ec.LenstraEdges.B1;
                                    return Math.Pow((double) ec.p, 0.25) < b1 && b1 <= Math.Pow((double) ec.p, 0.33);
                                }).ToList(),
                                ThirdClass = ecSubList.Where(ec =>
                                {
                                    var b1 = (double) ec.LenstraEdges.B1;
                                    return Math.Pow((double) ec.p, 0.33) < b1 && b1 <= Math.Pow((double) ec.p, 0.5);
                                }).ToList(),
                                ForthClass = ecSubList.Where(ec =>
                                {
                                    var b1 = (double) ec.LenstraEdges.B1;
                                    return b1 > Math.Pow((double) ec.p, 0.5);
                                }).ToList()
                            };
                            File.AppendAllText("IshmB.txt",
                                r.Dimension + " " + r.FirstClass.Count + " " + r.SecondClass.Count + " " +
                                r.ThirdClass.Count + " " + r.ForthClass.Count + "\t\r\n");
                            return r;
                        }).ToList();
                }
            }

            #endregion
        }
    }
}
