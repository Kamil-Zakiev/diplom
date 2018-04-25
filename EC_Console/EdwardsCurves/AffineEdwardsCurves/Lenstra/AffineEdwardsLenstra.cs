namespace EdwardsCurves.AffineEdwardsCurves.Lenstra
{
    using System;
    using System.Numerics;
    using System.Threading;
    using LenstraAlgorithm;
    using LenstraAlgorithm.Dto;
    using Utils;

    /// <summary> Алгоритм Ленстры на кривых Эдвардса в аффинных координатах </summary>
    /// <inheritdoc cref="ILenstra" />
    public class AffineEdwardsLenstra : ILenstra
    {
        public BigInteger B1 = BigInteger.Parse("100000");

        public LenstraFactorizationResult GetDivider(BigInteger n, Random random)
        {
            return GetDividerWithCancel(n, random, CancellationToken.None);
        }

        public LenstraFactorizationResult GetDividerWithCancel(BigInteger n, Random random, CancellationToken token)
        {
            var startTime = DateTime.Now;

            BigInteger x, y, d;
            do
            {
                x = BigIntegerExtensions.GetNextRandom(random, n);
                y = BigIntegerExtensions.GetNextRandom(random, n);
                d = ((x * x + y * y - 1) * (x * x * y * y).Inverse(n)).Mod(n);
            } while (d == 1 || d == 0);

            var edwardsCurve = new AffineEdwardsCurve(d, n);
            var pointsFactory = new PointsFactory(edwardsCurve);

            var calculator = new AffineEdwardsCurvePointCalculator();
            var point1 = pointsFactory.CreatePoint(x, y);

            BigInteger p = 2;

            try
            {
                while (p < B1 && !token.IsCancellationRequested)
                {
                    var pr = p;
                    while (pr < B1 && !token.IsCancellationRequested)
                    {
                        point1 = calculator.Mult(p, point1);
                        var gcd = BigInteger.GreatestCommonDivisor(point1.ParameterX, n);
                        if (gcd != BigInteger.One)
                        {
                            throw new GcdFoundException(gcd);
                        }

                        pr *= p;
                    }

                    p = BigIntegerExtensions.NextPrimaryMillerRabin(p);
                }
            }
            catch (GcdFoundException exc)
            {
                return new LenstraFactorizationResult
                {
                    EllepticCurve = edwardsCurve,
                    TargetNumber = n,
                    Divider = exc.GreatestCommonDivisor,
                    WastedTime = DateTime.Now - startTime,
                    EndType = EEndType.RunToComplete
                };
            }

            return new LenstraFactorizationResult
            {
                EllepticCurve = edwardsCurve,
                TargetNumber = n,
                WastedTime = DateTime.Now - startTime,
                EndType = token.IsCancellationRequested ? EEndType.Cancelled : EEndType.RunToComplete
            };
        }
    }
}