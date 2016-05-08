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

        public static List<LenstraResultOfEllepticCurve> UseThreadsParallelism(string pathTwoPrimesMultiple)
        {
            var twoPrimeMultiplesStrings = File.ReadAllLines(pathTwoPrimesMultiple);
            var tasks = new Task<LenstraResultOfEllepticCurve>[10];
            var ellepricCurves = new EllepticCurve[tasks.Length];
            var list = new List<LenstraResultOfEllepticCurve>();
            foreach (var n in twoPrimeMultiplesStrings.Select(BigInteger.Parse).Take(10))
            {
                for (int i = 0; i < tasks.Length; i++)
                    tasks[i] = Task.Factory.StartNew(() => Lenstra.GetDivider(n, _random));

                Task.WaitAll(tasks);
                list.AddRange(tasks.Select(task => task.Result));
            }

            return list;
        } 
    }
}
