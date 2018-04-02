using System;
using EdwardsCurves;

namespace TestProject
{
    internal class Program
    {
        public static void Main(string[] args)
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
    }
}