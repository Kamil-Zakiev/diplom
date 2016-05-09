using System.Collections.Generic;
using System.Linq;

namespace EC_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var results = new List<LenstraResultOfEllepticCurve>();
            for (int i = 10; i <= 20; i=i+2)
            {
                var path = string.Format("TwoPrimesMultiple/{0}digitsNumbers.txt", i);
                results.AddRange(MultithreadLenstra.UseThreadsParallelism(path, threadsCount: 10));
            }

            var groupResult = results.GroupBy(x => x.DividerDigitsCount).Select(y => new
            {
                countSuccess = y.Count(z => z.Success),
                countFailure = y.Count(z => !z.Success),
                averageSeconds = y.Average(z => z.WastedTime.TotalSeconds),
                maxSeconds = y.Max(z => z.WastedTime.TotalSeconds),
                minSeconds = y.Min(z => z.WastedTime.TotalSeconds)
            });
        }
    }
}
