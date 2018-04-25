namespace Analisys
{
    using System;
    using System.IO;
    using System.Linq;
    using Classificators;

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
            var constantsBasedClassificator = ClassificatorFactory.Create(new Func<CurveBaseInfo, bool>[]
            {
                info => info.EdgeB <= 100,
                info => info.EdgeB > 100 && info.EdgeB <= 1000,
                info => info.EdgeB > 1000 && info.EdgeB <= 10000,
                info => info.EdgeB > 10000
            });

            DoResearch(constantsBasedClassificator);
        }

        /// <summary> Исследование классов эллиптических кривых на основе их границы B относительно размености поля </summary>
        private static void ResearchFieldOrderBasedClasses()
        {
            Console.WriteLine("ResearchFieldOrderBasedClasses:");
            var fieldOrderBasedClassificator = ClassificatorFactory.Create(new Func<CurveBaseInfo, bool>[]
            {
                info => info.EdgeB <= Math.Pow(info.FieldOrder, 0.25),
                info => info.EdgeB > Math.Pow(info.FieldOrder, 0.25) && info.EdgeB <= Math.Pow(info.FieldOrder, 0.33),
                info => info.EdgeB > Math.Pow(info.FieldOrder, 0.33) && info.EdgeB <= Math.Pow(info.FieldOrder, 0.5),
                info => info.EdgeB > Math.Pow(info.FieldOrder, 0.5)
            });

            DoResearch(fieldOrderBasedClassificator);
        }

        private static void DoResearch(Classificator classificator)
        {
            var data = GetDataLines();

            var curveInfos = GetCurvesInfos(data);
            var classifiedCurveInfos = curveInfos.Select(curveInfo => new
                {
                    curveInfo,
                    curveClass = classificator.Classify(curveInfo)
                })
                .ToArray();

            var classesOfCurves = classifiedCurveInfos.GroupBy(info => info.curveInfo.DigitsCountOfFiledOrder)
                .OrderBy(g => g.Key)
                .Select(g =>
                {
                    var classesInfo = g
                        .AsEnumerable()
                        .GroupBy(cci => cci.curveClass.ClassNumber)
                        .OrderBy(gr => gr.Key)
                        .Select(gr => new
                        {
                            ClassNumber = gr.Key,
                            Count = gr.Count()
                        }).ToDictionary(x => x.ClassNumber, x => x.Count);

                    return new
                    {
                        Dimension = g.Key,
                        ClassesInfo = classesInfo
                    };
                })
                .ToArray();

            var classesNumber = classesOfCurves.Max(x => x.ClassesInfo.Count);
            var paddingLeft = 8;
            Console.Write("Dim".PadLeft(paddingLeft, ' '));
            for (var i = 0; i < classesNumber; i++)
            {
                Console.Write($"#{i + 1}".PadLeft(paddingLeft, ' '));
            }

            Console.WriteLine();

            foreach (var classesOfCurve in classesOfCurves)
            {
                Console.Write($"{classesOfCurve.Dimension}".PadLeft(paddingLeft, ' '));
                for (var i = 0; i < classesNumber; i++)
                {
                    if (classesOfCurve.ClassesInfo.ContainsKey(i + 1))
                    {
                        Console.Write($"{classesOfCurve.ClassesInfo[i + 1]}".PadLeft(paddingLeft, ' '));
                    }
                    else
                    {
                        Console.Write("0".PadLeft(paddingLeft, ' '));
                    }
                }

                Console.WriteLine();
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