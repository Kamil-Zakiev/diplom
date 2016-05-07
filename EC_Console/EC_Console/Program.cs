using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EC_Console
{
    class Program
    {
        private static Lenstra Lenstra = new Lenstra();
        private static Random _random = new Random();

        static void Main(string[] args)
        {
            var twoPrimeMultiplesStrings = File.ReadAllLines("Resource/TwoPrimesMultiple.txt");
            var tasks = new Task<LenstraResultOfEllepticCurve>[10];
            var list = new List<LenstraResultOfEllepticCurve>();
            foreach (var twoPrimeMultiplesString in twoPrimeMultiplesStrings)
            {
                var n = BigInteger.Parse(twoPrimeMultiplesString);
                for (int i = 0; i < tasks.Length; i++)
                    tasks[i] = Task.Factory.StartNew(() => Lenstra.GetDivider(n, _random));
                
                Task.WaitAll(tasks);
                list.AddRange(tasks.Select(task => task.Result));
            }
            var result = list.GroupBy(x => new
            {
                x.TargetNumber
            }).Select(y => new
            {
                number = y.Key,
                countSuccess = y.Count(z => z.Success),
                countFailure = y.Count(z => !z.Success),
                averageSeconds = y.Average(z => z.WastedTime.TotalSeconds),
                maxSeconds = y.Max(z => z.WastedTime.TotalSeconds),
                minSeconds = y.Max(z => z.WastedTime.TotalSeconds)
            });
        }
    }
}
