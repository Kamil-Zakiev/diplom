using System;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace EC_Console
{
    class Program
    {
        private static Lenstra Lenstra = new Lenstra();
        private static BigInteger n;

        private static Random random = new Random();
        private static BigInteger gcd;

        static void Main(string[] args)
        {
            #region Для покрытия тестами
            //var ec = new EllepticCurve(1, 1, 23);
            //var p1 = ec.CreatePoint(0, 1);
            //var p2 = ec.Sum(p1, p1);

            //for (int i = 0; i < 100; i++)
            //{
            //    p2 = ec.Sum(p2, p1);
            //    if(!p2.Equals(ec.Mult(i+3,p1)))
            //        throw new Exception("qqweqweqwe");
            //    Console.WriteLine(p2);
            //}
            #endregion


            var twoPrimeMultiplesStrings = File.ReadAllLines("Resource/TwoPrimesMultiple.txt");
            Task[] tasks = new Task[24];
            foreach (var twoPrimeMultiplesString in twoPrimeMultiplesStrings)
            {
                n = BigInteger.Parse(twoPrimeMultiplesString);
                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = Task.Factory.StartNew(test);
                }
                Task.WaitAny(tasks);
                //for (int i = 0; i < tasks.Length; i++)
                //{
                //    if(!tasks[i].IsCompleted)
                //        tasks[i].Dispose();
                //}
            }


        }

        public static void test()
        {
            var divider = Lenstra.GetDivider(n, random);
            if (divider != BigInteger.One)
                gcd = divider;
        }
    }
}
