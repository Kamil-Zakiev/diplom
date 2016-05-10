using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace EC_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 10; i <= 40; i = i + 2)
            {
                var path = string.Format("TwoPrimesMultiple/{0}digitsNumbers.txt", i);
                TwoPrimesMultipleGenerator.GenerateTwoPrimesMultipleNumbersInFile(path, i/2);
            }

            var results = new List<LenstraResultOfEllepticCurve>();
            for (int i = 10; i <= 40; i=i+2)
            {
                var path = string.Format("TwoPrimesMultiple/{0}digitsNumbers.txt", i);
                results.AddRange(MultithreadLenstra.UseThreadsParallelism(path, threadsCount: 10));
            }

            var groupResult = results
                .Where(x => x.Success)
                .GroupBy(x => x.DividerDigitsCount,
                (key, resultsOfGroup) =>
                {
                    var r = resultsOfGroup.ToList();
                    var averageSecondsOfSuccess = r.Average(z => z.WastedTime.TotalSeconds);
                    var maxSeconds = r.Max(z => z.WastedTime.TotalSeconds);
                    var minSeconds = r.Min(z => z.WastedTime.TotalSeconds);
                    var hist = new int[10];
                    r.Select(x => x.WastedTime.TotalSeconds).OrderBy(x => x).ToList()
                        .ForEach(time =>
                        {
                            var i = (int) (time/maxSeconds*9);
                            hist[i]++;
                        });
                    return new
                    {
                        dividerDigits = key,
                        hist = hist,
                        averageSecondsOfSuccess = averageSecondsOfSuccess,
                        maxSeconds = maxSeconds,
                        minSeconds = minSeconds
                    };
                }).ToList();
            var strList = groupResult.Select(x => x.dividerDigits + "\t" + string.Join(",", x.hist));
            File.WriteAllLines("Result.txt", strList, Encoding.UTF8);
        }
    }
}
