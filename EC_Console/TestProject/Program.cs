using System;
using System.Diagnostics;
using System.Numerics;
using EC_Console;
using EdwardsCurves;
using EdwardsCurves.Lenstra;
using ExtraUtils;
using Utils;

namespace TestProject
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //EdwardCurvesTest.Start();
           // Test8();
            Test6();
            Test7();
        }

        private static void Test1()
        {
            var d = 8;
            var fieldOrder = 13;

            var projectiveEdwardsCurve = new ProjectiveEdwardsCurve(d, fieldOrder);
            var pointsFactory = new PointsFactory(projectiveEdwardsCurve);

            var point1 = pointsFactory.CreatePoint(3, 6, 1);
            var point2 = pointsFactory.CreatePoint(6, 3, 1);

            var calculator = new ProjectiveEdwardsCurvePointCalculator();

            var sum = calculator.Sum(point1, point2);

            Console.WriteLine(sum);
            Console.WriteLine(sum.ToEdwardsCurvePoint());
        }

        private static void Test2()
        {
            const int d = 8;
            const int fieldOrder = 13;

            var projectiveEdwardsCurve = new ProjectiveEdwardsCurve(d, fieldOrder);
            var pointsFactory = new PointsFactory(projectiveEdwardsCurve);

            var point1 = pointsFactory.CreatePoint(6, 12, 2);
            var point2 = projectiveEdwardsCurve.NeitralPoint;

            var calculator = new ProjectiveEdwardsCurvePointCalculator();

            var sum = calculator.Sum(point1, point2);

            Console.WriteLine(sum);
            Console.WriteLine(sum.ToEdwardsCurvePoint());
        }
        
        private static void Test3()
        {
            for (var multipleCount = 2; multipleCount < 9999; multipleCount++)
            {
                Test3Inner(multipleCount);
            }
        }

        private static void Test3Inner(int multipleCount)
        {
            const int d = 8;
            const int fieldOrder = 13;
            var projectiveEdwardsCurve = new ProjectiveEdwardsCurve(d, fieldOrder);
            var pointsFactory = new PointsFactory(projectiveEdwardsCurve);

            var initialPoint = pointsFactory.CreatePoint(6, 12, 2);
            var point1 = pointsFactory.CreatePoint(6, 12, 2);

            var calculator = new ProjectiveEdwardsCurvePointCalculator();
            for (var i = 0; i < multipleCount - 1; i++)
            {
                point1 = calculator.Sum(point1, initialPoint);
            }

            var point2 = calculator.Mult(multipleCount, initialPoint);

            if (point1.Equals(point2))
            {
                Console.Write(".");
                return;
            }

            throw new ArithmeticException("Точки, полученные сложением и умножение, отличаются при multipleCount = "
                                          + multipleCount);
        }

        private static void Test4()
        {
            BigInteger fieldOrder = BigInteger.Parse("73928303")*BigInteger.Parse("73928293");
            BigInteger x = 3;
            BigInteger y = 2;
            BigInteger z = 1;

            var d = ((x * x * z * z + y * y * z * z - z * z * z * z) * (x * x * y * y).Inverse(fieldOrder)).Mod(fieldOrder);
            
            const int multipleCount = 73928303;

            var projectiveEdwardsCurve = new ProjectiveEdwardsCurve(d, fieldOrder);
            var pointsFactory = new PointsFactory(projectiveEdwardsCurve);

            var calculator = new ProjectiveEdwardsCurvePointCalculator();
            var initialPoint = pointsFactory.CreatePoint(x, y, z);
            var point1 = pointsFactory.CreatePoint(x, y, z);
            for (var i = 0; i < multipleCount - 1; i++)
            {
                point1 = calculator.Sum(point1, initialPoint);

                var gcd = BigInteger.GreatestCommonDivisor(point1.ParameterZ, fieldOrder);
                if (gcd != BigInteger.One)
                {
                    Console.WriteLine("divider = " + gcd);
                    return;
                }
                
              //  Console.WriteLine(point1);
              //  Console.WriteLine(point1.ToEdwardsCurvePoint());
            }
        }
        
        private static void Test5()
        {
            BigInteger fieldOrder = BigInteger.Parse("73928303")*BigInteger.Parse("73928293");
            BigInteger x = 3;
            BigInteger y = 2;
            BigInteger z = 1;

            var d = ((x * x * z * z + y * y * z * z - z * z * z * z) * (x * x * y * y).Inverse(fieldOrder)).Mod(fieldOrder);

            var projectiveEdwardsCurve = new ProjectiveEdwardsCurve(d, fieldOrder);
            var pointsFactory = new PointsFactory(projectiveEdwardsCurve);

            var calculator = new ProjectiveEdwardsCurvePointCalculator();
            var point1 = pointsFactory.CreatePoint(x, y, z);
            
            var b1 = 73928303;
            BigInteger p = 2;
            while (p < b1)
            {
                var pr = p;
                while (pr < b1)
                {
                    point1 = calculator.Mult(p, point1);
                    
                    var gcd = BigInteger.GreatestCommonDivisor(point1.ParameterZ, fieldOrder);
                    if (gcd != BigInteger.One)
                    {
                        Console.WriteLine("divider = " + gcd);
                        return;
                    }
                    
                    pr *= p;
                }
                p = BigIntegerExtensions.NextPrimaryMillerRabin(p);
            }
        }

        private static void Test6()
        {
            Console.WriteLine("ProjectiveEdwardsLenstra results: ");
            var n = BigInteger.Parse("73928303")*BigInteger.Parse("73928293");
            var multThreadLenstra = new MultithreadLenstra(new ProjectiveEdwardsLenstra());

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var res = multThreadLenstra.LenstraMultiThreadFastResult(n, 160);
            stopWatch.Stop();
            
            Console.WriteLine(res);
            Console.WriteLine("Elapsed ms: " + stopWatch.ElapsedMilliseconds);
        }

        private static void Test7()
        {
            Console.WriteLine("ClassicLenstra results: ");
            var n = BigInteger.Parse("73928303")*BigInteger.Parse("73928293");
            var multThreadLenstra = new MultithreadLenstra(new ClassicLenstra());
            
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var res = multThreadLenstra.LenstraMultiThreadFastResult(n, 160);
            stopWatch.Stop();

            Console.WriteLine(res);
            Console.WriteLine("Elapsed ms: " + stopWatch.ElapsedMilliseconds);
        }
      
        private static void Test8()
        {
            // BigInteger fieldOrder = BigInteger.Parse("73928303")*BigInteger.Parse("73928293");
            BigInteger fieldOrder = BigInteger.Parse("73928303")*BigInteger.Parse("73928293");
            BigInteger x, y, d;
            var random = new Random();
            do
            {
                x = BigIntegerExtensions.GetNextRandom(random, fieldOrder);
                y = BigIntegerExtensions.GetNextRandom(random, fieldOrder);
                d = ((x * x + y * y - 1) * (x * x * y * y).Inverse(fieldOrder)).Mod(fieldOrder);
            } while (d == 1 || d == 0);

            var edwardsCurve = new EdwardsCurve(d, fieldOrder);
            var pointsFactory = new PointsFactory(edwardsCurve);

            var calculator = new EdwardsCurvePointCalculator();
            var point1 = pointsFactory.CreatePoint(x, y);
            
            var b1 = 100000;
            BigInteger p = 2;
            
            try
            {
                while (p < b1)
                {
                    var pr = p;
                    while (pr < b1)
                    {
                        point1 = calculator.Mult(p, point1);
                        pr *= p;
                    }
                    p = BigIntegerExtensions.NextPrimaryMillerRabin(p);
                }
            }
            catch (GcdFoundException exc)
            {
                Console.WriteLine(exc.GreatestCommonDivisor);
            }

        }
    }
}