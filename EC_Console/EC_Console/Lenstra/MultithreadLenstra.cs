using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace EC_Console
{
    public class MultithreadLenstra
    {
        private static Lenstra Lenstra = new Lenstra();
        private static Random _random = new Random();

        /// <summary> Применяет пул ЭК к числам для факторизации </summary>
        /// <param name="pathTwoPrimesMultiple">Имя файла с числами для факторизации</param>
        /// <param name="threadsCount">Колчиство потоков</param>
        /// <returns>Список результатов со всех ЭК</returns>
        public static List<LenstraResultOfEllepticCurve> UseThreadsParallelism(string pathTwoPrimesMultiple, int threadsCount)
        {
            var twoPrimeMultiplesStrings = File.ReadAllLines(pathTwoPrimesMultiple);
            var list = new List<LenstraResultOfEllepticCurve>();
            foreach (var n in twoPrimeMultiplesStrings.Select(BigInteger.Parse).Take(10))
            {
                list.AddRange(LenstraMultiThreadResults(n, threadsCount));
            }

            return list;
        }

        /// <summary> Факторизация методом Ленстры </summary>
        /// <param name="n">Число, которое необходимо факторизовать</param>
        /// <param name="threadsCount">Количество потоков</param>
        private static IEnumerable<LenstraResultOfEllepticCurve> LenstraMultiThreadResults(BigInteger n, int threadsCount)
        {
            if(threadsCount < 1)
                throw new Exception("Количество потоков не может быть < 1");

            var tasks = new Task<LenstraResultOfEllepticCurve>[threadsCount];
            for (int i = 0; i < tasks.Length; i++)
                tasks[i] = Task.Factory.StartNew(() => Lenstra.GetDivider(n, _random));
            Task.WaitAll(tasks);

            var result = tasks.Select(task => task.Result);
            return result;
        } 
    }
}
