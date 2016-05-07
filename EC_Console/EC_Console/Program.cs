using System;
using System.IO;
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
            foreach (var twoPrimeMultiplesString in twoPrimeMultiplesStrings)
            {
                var n = BigInteger.Parse(twoPrimeMultiplesString);
                for (int i = 0; i < tasks.Length; i++)
                    tasks[i] = Task.Factory.StartNew(() => Lenstra.GetDivider(n, _random));
                
                Task.WaitAll(tasks);

                for (int i = 0; i < tasks.Length; i++)
                {
                    Console.WriteLine(tasks[i].Result);
                }
                break;
            }
        }
    }
}
