using System;
using System.IO;
using System.Linq;

namespace Analisys
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ResearchConstantsBasedClasses();
            ResearchFieldOrderBasedClasses();
        }

        /// <summary> Исследование классов эллиптических кривых на основе их границы B относительно констант </summary>
        private static void ResearchConstantsBasedClasses()
        {
            Console.WriteLine("ResearchConstantsBasedClasses:");
            DoResearch(curveInfo => curveInfo.ConstantsBasedClass);
        }

        /// <summary> Исследование классов эллиптических кривых на основе их границы B относительно размености поля </summary>
        private static void ResearchFieldOrderBasedClasses()
        {
            Console.WriteLine("ResearchFieldOrderBasedClasses:");
            DoResearch(curveInfo => curveInfo.FieldOrderBasedClass);
        }

        private static void DoResearch(Func<CurveInfo, CurveClass> classSelector)
        {
            var data = GetDataLines();

            var curveInfos = GetCurvesInfos(data);

            var classesOfCurves = curveInfos.GroupBy(info => info.DigitsCountOfFiledOrder)
                .OrderBy(g => g.Key)
                .Select(g =>
                {
                    var curvesClasses = g.Select(classSelector).ToArray();

                    return new
                    {
                        Dimension = g.Key,
                        FirstClass = curvesClasses.Count(curveClass => curveClass == CurveClass.FirstClass),
                        SecondClass = curvesClasses.Count(curveClass => curveClass == CurveClass.SecondClass),
                        ThirdClass = curvesClasses.Count(curveClass => curveClass == CurveClass.ThirdClass),
                        FourthClass = curvesClasses.Count(curveClass => curveClass == CurveClass.FourthClass)
                    };
                })
                .ToArray();

            foreach (var classesOfCurve in classesOfCurves)
            {
                var desc = $"{classesOfCurve.Dimension}\t{classesOfCurve.FirstClass}\t" +
                           $"{classesOfCurve.SecondClass}\t{classesOfCurve.ThirdClass}\t{classesOfCurve.FourthClass}";
                Console.WriteLine(desc);
            }
        }

        private static CurveInfo[] GetCurvesInfos(string[] data)
        {
            return data.Select(row => new CurveInfo(row)).ToArray();
        }

        private static string[] GetDataLines()
        {
            const string fileName =
                @"E:\Stash\diplom\EC_Console\EC_Console\bin\Debug\curves with its B and points count.txt";
            return File.ReadAllLines(fileName).Skip(1).ToArray();
        }
    }
}