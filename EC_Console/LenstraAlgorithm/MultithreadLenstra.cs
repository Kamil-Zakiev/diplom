namespace LenstraAlgorithm
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto;

    /// <summary> Сервис, предоставляющий функционал, который использует метод Ленстры в многопоточном режиме </summary>
    public class MultithreadLenstra<TLenstra> where TLenstra : ILenstra, new()
    {
        private readonly ILenstra _lenstra;
        
        private readonly Random _random = new Random();

        public MultithreadLenstra()
        {
            _lenstra = new TLenstra();
        }

        /// <summary> Применяет пул ЭК к числам для факторизации </summary>
        /// <param name="pathTwoPrimesMultiple">Имя файла с числами для факторизации</param>
        /// <param name="threadsCount">Колчиство потоков</param>
        /// <returns>Список результатов со всех ЭК</returns>
        public List<LenstraFactorizationResult> UseThreadsParallelism(string pathTwoPrimesMultiple, int threadsCount)
        {
            var twoPrimeMultiplesStrings = File.ReadAllLines(pathTwoPrimesMultiple);
            var list = new List<LenstraFactorizationResult>();
            foreach (var n in twoPrimeMultiplesStrings.Select(BigInteger.Parse).Take(10))
            {
                list.AddRange(LenstraMultiThreadResults(n, threadsCount));
            }

            return list;
        }

        /// <summary>  </summary>
        /// <param name="pathTwoPrimesMultiple">Имя файла с числами для факторизации</param>
        /// <param name="threadsCount">Количество потоков</param>
        public List<FactorizeTimeResult> GetMinTimes(string pathTwoPrimesMultiple, int threadsCount)
        {
            var twoPrimeMultiplesStrings = File.ReadAllLines(pathTwoPrimesMultiple);
            var list = new List<FactorizeTimeResult>();
            foreach (var n in twoPrimeMultiplesStrings.Select(BigInteger.Parse).Take(10))
            {
                list.Add(GetLenstraMultiThreadFastResultSeconds(n, threadsCount));
            }

            return list;
        }

        /// <summary> Факторизация методом Ленстры. Каждая кривая пытается факторизовать число вне зависимости от остальных кривых. </summary>
        /// <param name="n">Число, которое необходимо факторизовать</param>
        /// <param name="threadsCount">Количество потоков == количество ЭК</param>
        public IReadOnlyList<LenstraFactorizationResult> LenstraMultiThreadResults(BigInteger n, int threadsCount)
        {
            if (n == BigInteger.One)
            {
                throw new Exception("LenstraMultiThreadResults: n == BigInteger.One");
            }

            if (threadsCount < 1)
            {
                throw new Exception("Количество потоков не может быть < 1");
            }

            var result = new List<LenstraFactorizationResult>();
            var cycles = threadsCount / Environment.ProcessorCount;
            var leftCycles = threadsCount % Environment.ProcessorCount;

            // todo: удалить после проверки
            if (cycles * Environment.ProcessorCount + leftCycles != threadsCount)
            {
                throw new InvalidOperationException("Неправильно расчитано количество циклов");
            }

            for (var k = 0; k < cycles; k++)
            {
                var tasks = new Task<LenstraFactorizationResult>[Environment.ProcessorCount];
                for (var i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = Task.Factory.StartNew(() => _lenstra.GetDivider(n, _random));
                }

                Task.WaitAll(tasks);

                result.AddRange(tasks.Select(task => task.Result));
            }

            if (leftCycles > 0)
            {
                var tasks = new Task<LenstraFactorizationResult>[leftCycles];
                for (var i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = Task.Factory.StartNew(() => _lenstra.GetDivider(n, _random));
                }

                Task.WaitAll(tasks);

                result.AddRange(tasks.Select(task => task.Result));
            }

            return result;
        }

        /// <summary> Найти делитель числа </summary>
        /// <param name="n">Число, делитель которого необходимо найти</param>
        /// <param name="threadsCount">Количество потоков == количество ЭК для факторизации</param>
        /// <returns>Делитель или null</returns>
        public BigInteger? LenstraMultiThreadFastResult(BigInteger n, int threadsCount)
        {
            var cycles = (threadsCount - 1) / Environment.ProcessorCount + 1;
            for (var k = 0; k < cycles; k++)
            {
                var cts = new CancellationTokenSource();
                var tasks = new Task<LenstraFactorizationResult>[Environment.ProcessorCount];
                for (var i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = Task.Factory.StartNew(() => _lenstra.GetDividerWithCancel(n, _random, cts.Token),
                        cts.Token);
                }

                Task.WaitAny(tasks);
                var successedTask =
                    tasks.FirstOrDefault(x => x.Status == TaskStatus.RanToCompletion && x.Result.Success);

                cts.Cancel();
                if (successedTask?.Result != null)
                {
                    return successedTask.Result.Divider;
                }
            }

            return null;
        }

        /// <summary> Время(сек.) нахождения делителя </summary>
        /// <param name="n">Число, делитель которого необходимо найти</param>
        /// <param name="threadsCount">Количество потоков == количество ЭК для факторизации</param>
        public FactorizeTimeResult GetLenstraMultiThreadFastResultSeconds(BigInteger n, int threadsCount)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var divider = LenstraMultiThreadFastResult(n, threadsCount);
            stopWatch.Stop();

            return new FactorizeTimeResult
            {
                Divider = divider,
                TimeSpan = TimeSpan.FromTicks(stopWatch.ElapsedTicks),
                FactorizedNumber = n
            };
        }
    }
}