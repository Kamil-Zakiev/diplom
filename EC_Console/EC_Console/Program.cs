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
        private static BigInteger n;

        private static Random _random = new Random();
        private static BigInteger gcd=1;

        static void Main(string[] args)
        {
            var twoPrimeMultiplesStrings = File.ReadAllLines("Resource/TwoPrimesMultiple.txt");
            Task[] tasks = new Task[10];
            foreach (var twoPrimeMultiplesString in twoPrimeMultiplesStrings)
            {
                n = BigInteger.Parse(twoPrimeMultiplesString);
                var start = DateTime.Now;
                using (var cts = new CancellationTokenSource())
                {
                    for (int i = 0; i < tasks.Length; i++)
                    {
                        tasks[i] = Task.Factory.StartNew(() => test(n, cts.Token, cts), cts.Token);
                    }

                    Task.WaitAny(tasks);
                    var end = DateTime.Now;

                    StringBuilder stringBuilder = new StringBuilder();
                    if (gcd != BigInteger.One)
                        stringBuilder.AppendLine(string.Format("Число {0} = {1} * {2}", n, gcd, n/gcd));
                    else
                        stringBuilder.AppendLine(string.Format("Число {0} не разложено", n));
                    stringBuilder.AppendLine(string.Format("Потрачено {0} секунд", (end - start).TotalSeconds));
                    stringBuilder.AppendLine("");

                    File.AppendAllText("Resource/Отчет.txt", stringBuilder.ToString());
                    Console.WriteLine("Потрачено {0} секунд\n", (end - start).TotalSeconds);
                }
            }
        }

        public static void test(BigInteger n, CancellationToken token, CancellationTokenSource cts)
        {
            var result = Lenstra.GetDivider(n, _random, token);
            if (result.Success && !token.IsCancellationRequested)
            {
                //Если нашли делитель, то запоминаем его и отменяем оставшиеся потоки
                gcd = result.Divider;
                cts.Cancel();
            }
        }
    }
}
