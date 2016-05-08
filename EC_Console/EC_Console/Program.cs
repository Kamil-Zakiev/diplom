using System.Linq;

namespace EC_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = MultithreadLenstra.UseThreadsParallelism("Resource/TwoPrimesMultiple.txt");

            var groupResult = list.GroupBy(x => 1).Select(y => new
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
