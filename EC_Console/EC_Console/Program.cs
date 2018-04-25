using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using Utils;

namespace EC_Console
{
    public class Program
    {
        public const string CurvesWithItsBAndPointsCountTxt = "curves with its B and points count.txt";

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
/*
            var results = new List<FactorizeTimeResult>();
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
                            return new FactorizeTimeResult()
                            {
                                DividerDimension = dim,
                                Time = averMin
                            };
                        }).ToList();
            }
*/
            //факторизация
                /*
                var results = new List<LenstraFactorizationResult>();
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
            }
        }
}
