using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace EC_Console
{
    public class MinTimeResult
    {
        public int TargetDimension;
        public int DividerDimension;
        public double? Time;
    }

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
            #region Практическая часть диплома. Эксперимент
            /*
            //генерирование
            for (int i = 5; i <= 50; i = i + 5)
            {
                var path = string.Format("TwoPrimesMultiple/{0}digitsNumbers.txt", i);
                TwoPrimesMultipleGenerator.GenerateTwoPrimesMultipleNumbersInFile(path, i / 2);
            }*/

            var results = new List<MinTimeResult>();
            foreach (var path in Directory.GetFiles("TwoPrimesMultiple"))
            {
                results = MultithreadLenstra.GetMinTimes(path, threadsCount: 100);
                var group = results
                    .Where(x => x.Time.HasValue)
                    .GroupBy(x => x.DividerDimension,
                        (dim, res) =>
                        {
                            var averMin = res.Average(x => x.Time.Value);
                            var resultStr = dim + " " + averMin + "\t\r\n";
                            File.AppendAllText("ExperimentResultsMinTime.txt", resultStr);
                            return new MinTimeResult()
                            {
                                DividerDimension = dim,
                                Time = averMin
                            };
                        }).ToList();
            }

            //факторизация
                /*
                var results = new List<LenstraResultOfEllepticCurve>();
                foreach (var path in Directory.GetFiles("TwoPrimesMultiple"))
                {
                    results = (MultithreadLenstra.UseThreadsParallelism(path, threadsCount: 100));

                    //обработка результатов
                    var groupResult = results
                        .GroupBy(x => x.TargetNumberDigitsCount,
                            (key, resultsOfGroup) =>
                            {
                                //размерность факторизуемого числа
                                var targetDim = key;

                                var r = resultsOfGroup.ToList();

                                //количество успешных кривых
                                var successed = r.Count(x => x.Success);

                                //кол-во неуспешных кривых
                                var failed = r.Count(x => !x.Success);

                                //среднее время успеха
                                var averageSecondsOfSuccess = r.Where(x => x.Success)
                                    .Average(z => z.WastedTime.TotalSeconds);

                                //минимальное время успеха
                                var minSeconds = r.Where(x => x.Success).Min(z => z.WastedTime.TotalSeconds);

                                //максимальное время успеха
                                var maxSeconds = r.Where(x => x.Success).Max(z => z.WastedTime.TotalSeconds);

                                //распределение времени успеха на 10 столибиков гистограммы
                                var hist = new int[10];
                                r.Where(x => x.Success).Select(x => x.WastedTime.TotalSeconds).OrderBy(x => x).ToList()
                                    .ForEach(time =>
                                    {
                                        var i = (int)(time / maxSeconds * 9);
                                        hist[i]++;
                                    });

                                var result = new
                                {
                                    targetDim = targetDim,
                                    successed = successed,
                                    failed = failed,
                                    averageSecondsOfSuccess = averageSecondsOfSuccess,
                                    minSeconds = minSeconds,
                                    maxSeconds = maxSeconds,
                                    hist = hist
                                };
                                string resultString = string.Format("{0} {1} {2} {3} {4} {5} {6}\t\r\n",
                                    targetDim, successed, failed, averageSecondsOfSuccess, minSeconds, maxSeconds,
                                    string.Join(" ", hist));
                                File.AppendAllText("ExperimentResults.txt", resultString);
                                return result;
                            }).ToList();
                }
                */
                #endregion


                #region Теоретическая часть диплома. Оценка классов ЭК в зависимости от размерности делителя
                /*
                var random = new Random();
                var listOfEC = new List<EllepticCurve>();
                const int curvesCountPerDim = 100;
                for (int pDim = 2; pDim < 10; pDim++)
                {
                    var dimen = BigInteger.Pow(10, pDim);
                    for (int i = 0; i < curvesCountPerDim; i++)
                    {
                        dimen = BigIntegerExtensions.NextPrimaryMillerRabin(dimen);
                        var a = BigIntegerExtensions.GetNextRandom(random, dimen);
                        var b = BigIntegerExtensions.GetNextRandom(random, dimen);
                        var ec = new EllepticCurve(a, b, dimen);
                        listOfEC.Add(ec);
                    }
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
                        File.AppendAllText("IshmB.txt",
                            r.Dimension + " " + r.FirstClass.Count + " " + r.SecondClass.Count + " " +
                            r.ThirdClass.Count + " " + r.ForthClass.Count + "\t\r\n");
                        return r;
                    }).ToList();
                */
                #endregion
            }
        }
}
