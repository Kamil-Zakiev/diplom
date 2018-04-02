using System;
using EdwardsCurves;

namespace TestProject
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Test3();
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
    }
}