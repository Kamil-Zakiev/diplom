using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using LenstraAlgorithm;

namespace EC_Console
{
    public class MultithreadLenstra
    {
        public MultithreadLenstra(ILenstra lenstra)
        {
            Lenstra = lenstra;
        }
        
        private ILenstra Lenstra;
        private Random _random = new Random();

        /// <summary> Применяет пул ЭК к числам для факторизации </summary>
        /// <param name="pathTwoPrimesMultiple">Имя файла с числами для факторизации</param>
        /// <param name="threadsCount">Колчиство потоков</param>
        /// <returns>Список результатов со всех ЭК</returns>
        public List<LenstraResultOfEllepticCurve> UseThreadsParallelism(string pathTwoPrimesMultiple, int threadsCount)
        {
            var twoPrimeMultiplesStrings = File.ReadAllLines(pathTwoPrimesMultiple);
            var list = new List<LenstraResultOfEllepticCurve>();
            foreach (var n in twoPrimeMultiplesStrings.Select(BigInteger.Parse).Take(10))
            {
                list.AddRange(LenstraMultiThreadResults(n, threadsCount));
            }

            return list;
        }

        /// <summary>  </summary>
        /// <param name="pathTwoPrimesMultiple">Имя файла с числами для факторизации</param>
        /// <param name="threadsCount">Колчиство потоков</param>
        public List<MinTimeResult> GetMinTimes(string pathTwoPrimesMultiple, int threadsCount)
        {
            var twoPrimeMultiplesStrings = File.ReadAllLines(pathTwoPrimesMultiple);
            var list = new List<MinTimeResult>();
            foreach (var n in twoPrimeMultiplesStrings.Select(BigInteger.Parse).Take(10))
            {
                list.Add(GetLenstraMultiThreadFastResultSeconds(n, threadsCount));
            }

            return list;
        }

        /// <summary> Факторизация методом Ленстры </summary>
        /// <param name="n">Число, которое необходимо факторизовать</param>
        /// <param name="threadsCount">Количество потоков</param>
        public IEnumerable<LenstraResultOfEllepticCurve> LenstraMultiThreadResults(BigInteger n, int threadsCount)
        {
            if (n == BigInteger.One)
                throw new Exception("LenstraMultiThreadResults: n == BigInteger.One");
            if (threadsCount < 1)
                throw new Exception("Количество потоков не может быть < 1");

            var tasks = new Task<LenstraResultOfEllepticCurve>[threadsCount];
            for (int i = 0; i < tasks.Length; i++)
                tasks[i] = Task.Factory.StartNew(() => Lenstra.GetDivider(n, _random));
            Task.WaitAll(tasks);

            var result = tasks.Select(task => task.Result);
            return result;
        }

        public BigInteger? LenstraMultiThreadFastResult(BigInteger n, int threadsCount)
        {
            //логических процессоров - 8
            int cycles = (threadsCount - 1) / Environment.ProcessorCount + 1;
            var tasks = new Task<LenstraResultOfEllepticCurve>[Environment.ProcessorCount];
            LenstraResultOfEllepticCurve result = null;
            for (int k = 0; k < cycles; k++)
            {
                using (var cts = new CancellationTokenWithDisposedState())
                {
                    for (int i = 0; i < tasks.Length; i++)
                        tasks[i] =
                            Task.Factory.StartNew(() => Lenstra.GetDividerWithCancel(n, _random, cts.Token),
                                cts.Token);

                    Task.WaitAny(tasks);
                    if (tasks.Any(x => x.Status == TaskStatus.RanToCompletion && x.Result.Success))
                    {
                        result =
                            tasks.First(x => x.Status == TaskStatus.RanToCompletion && x.Result.Success).Result;
                    }
                    cts.Cancel();
                    if (result != null) return result.Divider;
                }
            }
            return null;
        }

        /// <summary> Время(сек.) нахождения делителя </summary>
        public MinTimeResult GetLenstraMultiThreadFastResultSeconds(BigInteger n, int threadsCount)
        {
            var start = DateTime.Now;
            var divider = LenstraMultiThreadFastResult(n, threadsCount);
            var result = new MinTimeResult();
            if (divider.HasValue)
            {
                result.DividerDimension = divider.Value.ToString().Length;
                result.Time = (DateTime.Now - start).TotalSeconds;
                return result;
            }
            result.TargetDimension = n.ToString().Length;
            result.Time = null;
            return result;
        }


    }
}
