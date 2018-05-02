namespace PointsSumPerformanceAnalisys
{
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Threading.Tasks;
    using EC_Console;
    using EdwardsCurves;
    using EdwardsCurves.AffineEdwardsCurves;
    using EdwardsCurves.ProjectiveEdwardsCurves;
    using Utils;

    internal class Program
    {
        public static Random Random = new Random();
        
        public static void Main(string[] args)
        {
            var fieldOrder = GetSizedRandomPrime(10);
            var sumsCount = 10000;
            
            Compare(fieldOrder, sumsCount);
            Compare(fieldOrder, sumsCount*10);
            Compare(fieldOrder, sumsCount*100);
            
            Compare(GetSizedRandomPrime(10), sumsCount);
            Compare(GetSizedRandomPrime(10+3), sumsCount);
            Compare(GetSizedRandomPrime(10+6), sumsCount);
            
            /*
                Results of summing 10000 times on field with order 2845536623
                Weier: 91ms
                AffineEdwards: 148ms
                ProjEdwards: 54ms
                Results of summing 100000 times on field with order 2845536623
                Weier: 832ms
                AffineEdwards: 1360ms
                ProjEdwards: 382ms
                Results of summing 1000000 times on field with order 2845536623
                Weier: 8160ms
                AffineEdwards: 13505ms
                ProjEdwards: 3719ms
                Results of summing 10000 times on field with order 9690953449
                Weier: 105ms
                AffineEdwards: 175ms
                ProjEdwards: 63ms
                Results of summing 10000 times on field with order 8895348761251
                Weier: 159ms
                AffineEdwards: 279ms
                ProjEdwards: 71ms
                Results of summing 10000 times on field with order 1105857379773127
                Weier: 199ms
                AffineEdwards: 366ms
                ProjEdwards: 82ms
             */
            
        }

        private static void Compare(BigInteger fieldOrder, int sumsCount)
        {
            var tasks = new[]
            {
                new Task<(long, string)>(() => (GetMsOfWeierCurve(fieldOrder, sumsCount), "Weier")),
                new Task<(long, string)>(() => (GetMsOfAffineEdwardsCurve(fieldOrder, sumsCount), "AffineEdwards")),
                new Task<(long, string)>(() => (GetMsOfProjEdwardsCurve(fieldOrder, sumsCount), "ProjEdwards"))
            };
            foreach (var task in tasks)
            {
                task.Start();
            }

            Task.WaitAll(tasks);
            Console.WriteLine($"Results of summing {sumsCount} times on field with order {fieldOrder}");
            foreach (var task in tasks)
            {
                var curveType = task.Result.Item2;
                var ms = task.Result.Item1;
                Console.WriteLine(curveType + ": " + ms + "ms");
            }
        }

        private static BigInteger GetSizedRandomPrime(int size)
        {

            var topBound = BigInteger.Pow(10, size);
            var bottonBound = BigInteger.Pow(10, size - 1);
            BigInteger number = 0;
            while (number >= topBound || number <= bottonBound)
            {
                var randomNumber = BigIntegerExtensions.GetNextRandom(Random, BigInteger.Pow(10, size));
                number = BigIntegerExtensions.NextPrimaryMillerRabin(randomNumber);
            }

            return number;
        }
        
        public static long GetMsOfWeierCurve(BigInteger fieldOrder, int sumsCount)
        {
            var a = 5;
            var b = 3;
            var ec = new EllepticCurve(a, b, fieldOrder);

            BigInteger x = 0;
            var tenExp = 1000000000000000000;
            var y = Math.Sqrt((double) (x * x * x + a * x + b).Mod(fieldOrder));
            while (x < fieldOrder && ((long) (y * tenExp) % tenExp != 0))
            {
                x++;
                y = Math.Sqrt((double) (x * x * x + a * x + b).Mod(fieldOrder));
            }

            var point = ec.CreatePoint(x, (BigInteger) y);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 0; i < sumsCount; i++)
            {
               // Console.Write("\r"+(((double)i)/sumsCount).ToString("P"));
                point = ec.Sum(point, point);
            }

         //   Console.WriteLine();
            stopWatch.Stop();
            
            return stopWatch.ElapsedMilliseconds;

        }
        
        public static long GetMsOfAffineEdwardsCurve(BigInteger fieldOrder, int sumsCount)
        {
            BigInteger x, y, d;
            var random = new Random();
            do
            {
                x = BigIntegerExtensions.GetNextRandom(random, fieldOrder);
                y = BigIntegerExtensions.GetNextRandom(random, fieldOrder);
                d = ((x * x + y * y - 1) * (x * x * y * y).Inverse(fieldOrder)).Mod(fieldOrder);
            } while (d == 1 || d == 0);
           
            var edwardsCurve = new AffineEdwardsCurve(d, fieldOrder);
            
            var pointsFactory = new PointsFactory(edwardsCurve);
            var calculator = new AffineEdwardsCurvePointCalculator();

            var point = pointsFactory.CreatePoint(x, y);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 0; i < sumsCount; i++)
            {
                // Console.Write("\r"+(((double)i)/sumsCount).ToString("P"));
                point = calculator.Sum(point, point);
            }

            //Console.WriteLine();
            stopWatch.Stop();
            
            return stopWatch.ElapsedMilliseconds;
        }
        
        public static long GetMsOfProjEdwardsCurve(BigInteger fieldOrder, int sumsCount)
        {
            var random = new Random();
            BigInteger x, y, z, d;
            do
            { 
                x = BigIntegerExtensions.GetNextRandom(random, fieldOrder);
                y = BigIntegerExtensions.GetNextRandom(random, fieldOrder);
                z = BigIntegerExtensions.GetNextRandom(random, fieldOrder);
                d = ((x * x * z * z + y * y * z * z - z * z * z * z) * (x * x * y * y).Inverse(fieldOrder)).Mod(fieldOrder);
            } while (d == 1 || d == 0);
            
            var projectiveEdwardsCurve = new ProjectiveEdwardsCurve(d, fieldOrder);
            var pointsFactory = new PointsFactory(projectiveEdwardsCurve);
            
            var calculator = new ProjectiveEdwardsCurvePointCalculator();

            var point = pointsFactory.CreatePoint(x, y, z);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 0; i < sumsCount; i++)
            {
                // Console.Write("\r"+(((double)i)/sumsCount).ToString("P"));
                point = calculator.Sum(point, point);
            }

            //Console.WriteLine();
            stopWatch.Stop();
            
            return stopWatch.ElapsedMilliseconds;
        }
    }
}