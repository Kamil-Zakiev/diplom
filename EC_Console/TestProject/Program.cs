using System;
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
            Test6();
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
            var n = BigInteger.Parse("73928303")*BigInteger.Parse("73928293");
            var multThreadLenstra = new MultithreadLenstra(new EdwardsLenstra());
            var res = multThreadLenstra.LenstraMultiThreadFastResult(n, 160);

            Console.WriteLine(res);
        }

        private static void Test7()
        {
            var n = BigInteger.Parse("73928303")*BigInteger.Parse("73928293");
            var multThreadLenstra = new MultithreadLenstra(new ClassicLenstra());
            var res = multThreadLenstra.LenstraMultiThreadFastResult(n, 160);

            Console.WriteLine(res);
        }
    }
}