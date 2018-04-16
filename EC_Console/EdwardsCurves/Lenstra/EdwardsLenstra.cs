using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using ExtraUtils;
using LenstraAlgorithm;
using Utils;

namespace EdwardsCurves.Lenstra
{
    public class EdwardsLenstra : ILenstra
    {
        public BigInteger B1 = BigInteger.Parse("100000");

        /// <summary>  Возвращает объект LenstraResultOfEllepticCurve </summary>
        /// <param name="n">Число, у которого требуется найти делитель</param>
        public LenstraResultOfEllepticCurve GetDivider(BigInteger n, Random random)
        {
            var startTime = DateTime.Now;
            var x = BigIntegerExtensions.GetNextRandom(random, n);
            var y = BigIntegerExtensions.GetNextRandom(random, n);
            var z = BigIntegerExtensions.GetNextRandom(random, n);
            
            var d = ((x * x * z * z + y * y * z * z - z * z * z * z) * (x * x * y * y).Inverse(n)).Mod(n);

            var projectiveEdwardsCurve = new ProjectiveEdwardsCurve(d, n);
            var pointsFactory = new PointsFactory(projectiveEdwardsCurve);

            var calculator = new ProjectiveEdwardsCurvePointCalculator();
            var point1 = pointsFactory.CreatePoint(x, y, z);
            
            BigInteger p = 2;
            while (p < B1)
            {
                var pr = p;
                while (pr < B1)
                {
                    point1 = calculator.Mult(p, point1);
                    
                    var gcd = BigInteger.GreatestCommonDivisor(point1.ParameterZ, n);
                    if (gcd != BigInteger.One)
                    {
                        Console.WriteLine($"Поток {Task.CurrentId} молодец: {n} = {gcd} * {n / gcd}");
                        return new LenstraResultOfEllepticCurve
                        {
                            EllepticCurve = projectiveEdwardsCurve,
                            TargetNumber = n,
                            Divider = gcd,
                            WastedTime = DateTime.Now - startTime
                        };
                    }
                    
                    pr *= p;
                }
                p = BigIntegerExtensions.NextPrimaryMillerRabin(p);
            }

            return new LenstraResultOfEllepticCurve
            {
                EllepticCurve = projectiveEdwardsCurve,
                TargetNumber = n,
                WastedTime = DateTime.Now - startTime
            };
        }


        /// <summary>  
        /// Возвращает объект LenstraResultOfEllepticCurve, 
        /// если находится какое-то число, то все осатльные потоки прекращаются 
        /// </summary>
        /// <param name="n">Число, у которого требуется найти делитель</param>
        /// <param name="token">Токен отмены</param>
        public LenstraResultOfEllepticCurve GetDividerWithCancel(BigInteger n, Random random, CancellationToken token)
        {
            var startTime = DateTime.Now;
            var x = BigIntegerExtensions.GetNextRandom(random, n);
            var y = BigIntegerExtensions.GetNextRandom(random, n);
            var z = BigIntegerExtensions.GetNextRandom(random, n);
            
            var d = ((x * x * z * z + y * y * z * z - z * z * z * z) * (x * x * y * y).Inverse(n)).Mod(n);

            var projectiveEdwardsCurve = new ProjectiveEdwardsCurve(d, n);
            var pointsFactory = new PointsFactory(projectiveEdwardsCurve);

            var calculator = new ProjectiveEdwardsCurvePointCalculator();
            var point1 = pointsFactory.CreatePoint(x, y, z);
            
            BigInteger p = 2;
            while (p < B1 && !token.IsCancellationRequested)
            {
                var pr = p;
                while (pr < B1 && !token.IsCancellationRequested)
                {
                    point1 = calculator.Mult(p, point1);
                    
                    var gcd = BigInteger.GreatestCommonDivisor(point1.ParameterZ, n);
                    if (gcd != BigInteger.One)
                    {
                        Console.WriteLine($"Поток {Task.CurrentId} молодец: {n} = {gcd} * {n / gcd}");
                        return new LenstraResultOfEllepticCurve
                        {
                            EllepticCurve = projectiveEdwardsCurve,
                            TargetNumber = n,
                            Divider = gcd,
                            WastedTime = DateTime.Now - startTime
                        };
                    }
                    
                    pr *= p;
                }
                p = BigIntegerExtensions.NextPrimaryMillerRabin(p);
            }

            Console.WriteLine(token.IsCancellationRequested
                ? $"Поток {Task.CurrentId} остановлен"
                : $"Поток {Task.CurrentId} не смог");


            return new LenstraResultOfEllepticCurve
            {
                EllepticCurve = projectiveEdwardsCurve,
                TargetNumber = n,
                WastedTime = DateTime.Now - startTime
            };
        }


    }
}