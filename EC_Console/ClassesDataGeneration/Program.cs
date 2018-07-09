namespace ClassesDataGeneration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Numerics;
    using EC_Console;
    using Utils;

    internal class Program
    {
        public const string CurvesWithItsBAndPointsCountTxt = "curves with its B and points count.txt";

        public static void Main(string[] args)
        {
            var random = new Random();
            var listOfEc = new List<EllepticCurve>();
            const int curvesCountPerDim = 100;
            for (var pDim = 2; pDim < 10; pDim++)
            {
                var dimen = BigInteger.Pow(10, pDim);
                for (var i = 0; i < curvesCountPerDim; i++)
                {
                    dimen = BigIntegerExtensions.NextPrimaryMillerRabin(dimen);
                    var a = BigIntegerExtensions.GetNextRandom(random, dimen);
                    var b = BigIntegerExtensions.GetNextRandom(random, dimen);
                    var ec = new EllepticCurve(a, b, dimen);
                    listOfEc.Add(ec);
                }
            }

            foreach (var curve in listOfEc)
            {
                var pointsCount = curve.CountPoints;
                var b1 = curve.LenstraEdges.B1;

                var dim = curve.p.ToString().Length;
                var info = $"{dim}\t{curve.a}\t{curve.b}\t{curve.p}\t{pointsCount}\t{b1}\t\r\n";
                File.AppendAllText(CurvesWithItsBAndPointsCountTxt, info);
            }
        }
    }
}