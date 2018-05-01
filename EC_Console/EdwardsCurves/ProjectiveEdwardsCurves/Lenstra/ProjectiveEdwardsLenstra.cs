﻿namespace EdwardsCurves.ProjectiveEdwardsCurves.Lenstra
{
    using System;
    using System.Globalization;
    using System.Numerics;
    using System.Threading;
    using LenstraAlgorithm;
    using LenstraAlgorithm.Dto;
    using Utils;

    /// <summary> Алгоритм Ленстры на кривых Эдвардса в проективных координатах </summary>
    /// <inheritdoc cref="ILenstra" />
    public class ProjectiveEdwardsLenstra : ILenstra
    {
        public BigInteger B1 = BigInteger.Parse("100000");

        public LenstraFactorizationResult GetDivider(BigInteger n, Random random)
        {
            return GetDividerWithCancel(n, random, CancellationToken.None);
        }

        public LenstraFactorizationResult GetDividerWithCancel(BigInteger n, Random random, CancellationToken token)
        {
            var startTime = DateTime.Now;
            
            BigInteger x, y, z, d;
            do
            { 
                x = BigIntegerExtensions.GetNextRandom(random, n);
                y = BigIntegerExtensions.GetNextRandom(random, n);
                z = BigIntegerExtensions.GetNextRandom(random, n);
                d = ((x * x * z * z + y * y * z * z - z * z * z * z) * (x * x * y * y).Inverse(n)).Mod(n);
            } while (d == 1 || d == 0);
            
            var projectiveEdwardsCurve = new ProjectiveEdwardsCurve(d, n);
            PointsFactory pointsFactory;
            try
            {
                pointsFactory = new PointsFactory(projectiveEdwardsCurve);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("d = " + d);
                Console.WriteLine("x = " + x);
                Console.WriteLine("y = " + y);
                Console.WriteLine("z = " + z);
                throw;
            }

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
                    // пока это не будет реализовано
                    var gcd = BigInteger.GreatestCommonDivisor(point1.ParameterX, n);
                    if (gcd != BigInteger.One)
                    {
                        return new LenstraFactorizationResult
                        {
                            EllepticCurve = projectiveEdwardsCurve,
                            TargetNumber = n,
                            Divider = gcd,
                            WastedTime = DateTime.Now - startTime,
                            EndType = EEndType.RunToComplete
                        };
                    }
                }

                p = BigIntegerExtensions.NextPrimaryMillerRabin(p);
            }

            return new LenstraFactorizationResult
            {
                EllepticCurve = projectiveEdwardsCurve,
                TargetNumber = n,
                WastedTime = DateTime.Now - startTime,
                EndType = token.IsCancellationRequested ? EEndType.Cancelled : EEndType.RunToComplete
            };
        }
    }
}