using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using LenstraAlgorithm;
using Utils;

namespace EdwardsCurves.Lenstra
{
    using LenstraAlgorithm.Dto;

    public class ProjectiveEdwardsLenstra : ILenstra
    {
        public BigInteger B1 = BigInteger.Parse("1000000");

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
            BigInteger step = 2000;
            long iteration = 0;
            while (p < B1)
            {
                var pr = p;
                while (pr < B1)
                {
                    point1 = calculator.Mult(p, point1);

                    if (iteration++ % step != 0)
                    {
                        pr *= p;
                        continue;
                    }
                    
                    // вычисляем НОД только через каждые step итераций, т.к. НОД уменьшает производительность 
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
            BigInteger step = 1;
            long iteration = 0;
            while (p < B1 && !token.IsCancellationRequested)
            {
                var pr = p;
                while (pr < B1 && !token.IsCancellationRequested)
                {
                    point1 = calculator.Mult(p, point1);
                    
//                    if (iteration++ % step != 0)
//                    {
//                        pr *= p;
//                        continue;
//                    }

                    pr *= p;
                    if (point1.ParameterX == BigInteger.Zero)
                    {
                        continue;
                    }
                    
                    // вычисляем НОД только через каждые step итераций, т.к. НОД уменьшает производительность 
                    var gcd = BigInteger.GreatestCommonDivisor(point1.ParameterX, n);
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