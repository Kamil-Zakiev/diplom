using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using EdwardsCurves;
using ExtraUtils;
using Utils;

namespace TestProject
{
    public class EdwardCurvesTest
    {
        public static void TestSum(EdwardsCurvePoint point1, EdwardsCurvePoint point2)
        {
            var calculator = new EdwardsCurvePointCalculator();
            var p3 = calculator.Sum(point1, point2);
            
            var fieldOrder = point1.EdwardsCurve.FieldOrder;
            var d = point1.EdwardsCurve.ParameterD;
            CheckEquality(p3.ParameterX, p3.ParameterY, d, fieldOrder);

            point2 = new EdwardsCurvePoint((-point2.ParameterX).Mod(fieldOrder), point2.ParameterY, point2.EdwardsCurve); 
            p3 = new EdwardsCurvePoint((-p3.ParameterX).Mod(fieldOrder), p3.ParameterY, p3.EdwardsCurve); 

            var sum2 = calculator.Sum(point1, p3);
            CheckEquality(sum2.ParameterX, sum2.ParameterY, d, fieldOrder);

            if (sum2.ToString() == point2.ToString())
            {
                return;
            }

            throw new Exception("123");

        }

        public static void CheckSum()
        {
            BigInteger fieldOrder = 11;
            BigInteger d = 2;
            var edwardsCurve = new EdwardsCurve(d, fieldOrder);
            var pointsFactory = new PointsFactory(edwardsCurve);

            var points = new[]
            {
                CreatePoint(0, 1, pointsFactory),
                CreatePoint(0, 10, pointsFactory),
                CreatePoint(1, 0, pointsFactory),
                CreatePoint(3, 4, pointsFactory),
                CreatePoint(3, 7, pointsFactory),
                CreatePoint(4, 3, pointsFactory),
                CreatePoint(4, 8, pointsFactory),
                CreatePoint(7, 3, pointsFactory),
                CreatePoint(7, 8, pointsFactory),
                CreatePoint(8, 4, pointsFactory),
                CreatePoint(8, 7, pointsFactory),
                CreatePoint(10, 0, pointsFactory)
            };

            for (int i = 0; i < points.Length; i++)
            {
                for (int j = 0; j < points.Length; j++)
                {
                    TestSum(points[i], points[j]);
                }
            }
           
//            return;
//            var sum = CreatePoint(7, 3, pointsFactory);
//            
//            var calculator = new EdwardsCurvePointCalculator();
//            var p3 = calculator.Sum(point1, point2);
//            CheckEquality(p3.ParameterX, p3.ParameterY, d, fieldOrder);
//            
//            Console.WriteLine(point1);
//            Console.WriteLine(point2);
//            Console.WriteLine(p3);
        }

        private static EdwardsCurvePoint CreatePoint(BigInteger x, BigInteger y, PointsFactory pointsFactory)
        {
            var d = pointsFactory.EdwardsCurve.ParameterD;
            var fieldOrder = pointsFactory.EdwardsCurve.FieldOrder;
            CheckEquality(x, y, d, fieldOrder);

            return pointsFactory.CreatePoint(x, y);

        }

        private static void CheckEquality(BigInteger x, BigInteger y, BigInteger d, BigInteger fieldOrder)
        {
            var x2 = x * x;
            var y2 = y * y;
            var left = (x2 + y2).Mod(fieldOrder);
            var right = (1 + d * x2 * y2).Mod(fieldOrder);

            if (left == right)
            {
                return;
            }

            var errorMsg = $"Точка ({x}, {y}) не принадлежит кривой x^2+y^2 = 1 + {d}*x^2*y^2 mod {fieldOrder}";
            throw new InvalidOperationException(errorMsg);
        }

        public static void Start()
        {
            GetAllPoints();
        }

        public static void GetAllPoints()
        {
            BigInteger fieldOrder = BigInteger.Parse("113")*BigInteger.Parse("19");
            BigInteger x, y, d;
            var random = new Random();
            do
            {
                x = BigIntegerExtensions.GetNextRandom(random, fieldOrder);
                y = BigIntegerExtensions.GetNextRandom(random, fieldOrder);
                d = ((x * x + y * y - 1) * (x * x * y * y).Inverse(fieldOrder)).Mod(fieldOrder);
            } while (d == 1 || d == 0);

       //     d = 1;
            Console.WriteLine($"x = {x}");
            Console.WriteLine($"y = {y}");

            var edwardsCurve = new EdwardsCurve(d, fieldOrder);
            Console.WriteLine(edwardsCurve);
            
            var pointsFactory = new PointsFactory(edwardsCurve);
            
            var points = new List<EdwardsCurvePoint>();
            for (var x1 = 0; x1 < fieldOrder; x1++)
            {
                for (var y1 = 0; y1 < fieldOrder; y1++)
                {
                    if (!pointsFactory.SoftCheckPointOnCurve(x1, y1))
                    {
                        continue;
                    }

                    points.Add(pointsFactory.CreatePoint(x1, y1));
                }
            }

            Console.WriteLine("Edwards curve has " + points.Count + " points");

            var calculator = new EdwardsCurvePointCalculator();
            var pairs = points.Join(points, p1 => 1, p2 => 1, (p1, p2) => new {p1, p2}).ToArray();
            try
            {
                var sumPoints = pairs.AsParallel().Select(pair => calculator.Sum(pair.p1, pair.p2));
                Console.WriteLine("Sums were calculated without any exception. Sums count:" + sumPoints.Count());
            }
            catch (AggregateException e)
            {
                var gcdFoundException = e.InnerExceptions.First() as GcdFoundException;
                if (gcdFoundException == null)
                {
                    throw;
                }
                
                Console.WriteLine(gcdFoundException.Message);
                Console.WriteLine(gcdFoundException.GreatestCommonDivisor);
            }
        }
        
        public static void GetAllPointsSpecial2()
        {
            BigInteger fieldOrder = BigInteger.Parse("7")*BigInteger.Parse("5");
            BigInteger d = 3;
            
            var edwardsCurve = new EdwardsCurve(d, fieldOrder);
            var pointsFactory = new PointsFactory(edwardsCurve);
            var points = new List<EdwardsCurvePoint>();
            for (var x1 = 0; x1 < fieldOrder; x1++)
            {
                for (var y1 = 0; y1 < fieldOrder; y1++)
                {
                    if (!pointsFactory.SoftCheckPointOnCurve(x1, y1))
                    {
                        continue;
                    }

                    points.Add(pointsFactory.CreatePoint(x1, y1));
                }
            }

            Console.WriteLine("Edwards curve has " + points.Count + " points");

            var calculator = new EdwardsCurvePointCalculator();
            var pairs = points.Join(points, p1 => 1, p2 => 1, (p1, p2) => new {p1, p2}).ToArray();
            try
            {
                var sumPoints = pairs.AsParallel().Select(pair => calculator.Sum(pair.p1, pair.p2));
                Console.WriteLine("Sums were calculated without any exception. Sums count:" + sumPoints.Count());
            }
            catch (AggregateException e)
            {
                var gcdFoundException = e.InnerExceptions.First() as GcdFoundException;
                if (gcdFoundException == null)
                {
                    throw;
                }
                
                Console.WriteLine(gcdFoundException.Message);
                Console.WriteLine(gcdFoundException.GreatestCommonDivisor);
            }
        }

        public static void GetAllPointsSpecial()
        {
            BigInteger fieldOrder = BigInteger.Parse("113")*BigInteger.Parse("19");
            BigInteger x = 705, y = 232, d = 1577;
            var edwardsCurve = new EdwardsCurve(d, fieldOrder);
            
            var pointsFactory = new PointsFactory(edwardsCurve);
            var points = new List<EdwardsCurvePoint>();
            for (var x1 = 0; x1 < fieldOrder; x1++)
            {
                for (var y1 = 0; y1 < fieldOrder; y1++)
                {
                    if (!pointsFactory.SoftCheckPointOnCurve(x1, y1))
                    {
                        continue;
                    }

                    points.Add(pointsFactory.CreatePoint(x1, y1));
                }
            }

            Console.WriteLine("Edwards curve has " + points.Count + " points");

            var calculator = new EdwardsCurvePointCalculator();
            var pairs = points.Join(points, p1 => 1, p2 => 1, (p1, p2) => new {p1, p2}).ToArray();
            
            
            try
            {
                var sumPoints = pairs.AsParallel().Select(pair => calculator.Sum(pair.p1, pair.p2));
                Console.WriteLine("Sums were calculated without any exception. Sums count:" + sumPoints.Count());
            }
            catch (AggregateException e)
            {
                var gcdFoundException = e.InnerExceptions.First() as GcdFoundException;
                if (gcdFoundException == null)
                {
                    throw;
                }
                
                Console.WriteLine(gcdFoundException.Message);
                Console.WriteLine(gcdFoundException.GreatestCommonDivisor);
            }
        }
    }
}