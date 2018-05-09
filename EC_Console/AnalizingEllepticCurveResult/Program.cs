namespace AnalizingEllepticCurveResult
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using System.Text.RegularExpressions;
    using EC_Console;
    using EdwardsCurves.AffineEdwardsCurves.Lenstra;
    using EdwardsCurves.ProjectiveEdwardsCurves.Lenstra;

    /// <summary> Анализ результата работы каждой ЭК </summary>
    internal class Program
    {
        private static readonly Dictionary<string, List<ParsedLenstraFactorizationResult>> Dictionary;

        static Program()
        {
            Dictionary = new Dictionary<string, List<ParsedLenstraFactorizationResult>>();
        }

        public static void Main(string[] args)
        {
            PrintSuccessedInfo();
            PrintStatistics();
        }

        private static void PrintStatistics()
        {
            var lenstraVersions = new[]
                {nameof(ClassicLenstra), nameof(AffineEdwardsLenstra), nameof(ProjectiveEdwardsLenstra)};

            foreach (var lenstraVersion in lenstraVersions)
            {
                Console.WriteLine(lenstraVersion + ":");
                var statisticOfVersion = GetStatistics(lenstraVersion).OrderBy(t => t.DividerDigitsCount).ToArray();
                var paddingLeft = 15;
                Console.Write("DivDim".PadLeft(paddingLeft));
                Console.Write("MinMs".PadLeft(paddingLeft));
                Console.Write("AverMs".PadLeft(paddingLeft));
                Console.Write("MaxMs".PadLeft(paddingLeft));
                Console.WriteLine();
                foreach (var time in statisticOfVersion.OrderBy(info => info.DividerDigitsCount))
                {
                    Console.Write($"{time.DividerDigitsCount}".PadLeft(paddingLeft));
                    Console.Write($"{time.MinMs:F2}".PadLeft(paddingLeft));
                    Console.Write($"{time.AverageMs:F2}".PadLeft(paddingLeft));
                    Console.Write($"{time.MaxMs:F2}".PadLeft(paddingLeft));
                    Console.WriteLine();
                }

                Console.WriteLine("Histogram of success timing:");
                foreach (var time in statisticOfVersion)
                {
                    Console.Write($"{time.DividerDigitsCount}:".PadLeft(paddingLeft));
                    Console.WriteLine($"{string.Join("", time.Hist.Select(h => h.ToString().PadLeft(paddingLeft)))}");
                }

                Console.WriteLine();
            }
        }

        private static Statistic[] GetStatistics(string lenstraVersion)
        {
            var allLenstraResults = GetAllLenstraResults(lenstraVersion);
            var statisticOfVersion = allLenstraResults
                .Where(x => x.Success)
                .GroupBy(r => r.DividerDigitsCount)
                .Select(g =>
                {
                    var lenstraMsResults = g.Select(x => x.WastedTime.TotalMilliseconds).ToArray();

                    var averageMs = lenstraMsResults.Average();
                    var minMs = lenstraMsResults.Min();
                    var maxMs = lenstraMsResults.Max();

                    var hist = new int[10];
                    foreach (var ms in lenstraMsResults)
                    {
                        var i = (int) Math.Floor((ms - minMs) / (maxMs - minMs) * 10);
                        if (i == 10)
                        {
                            i--;
                        }

                        hist[i]++;
                    }

                    return new Statistic
                    {
                        DividerDigitsCount = g.Key,
                        MinMs = minMs,
                        AverageMs = averageMs,
                        MaxMs = maxMs,
                        Hist = hist
                    };
                })
                .ToArray();
            return statisticOfVersion;
        }

        private static void PrintSuccessedInfo()
        {
            var lenstraVersions = new[]
                {nameof(ClassicLenstra), nameof(AffineEdwardsLenstra), nameof(ProjectiveEdwardsLenstra)};

            foreach (var lenstraVersion in lenstraVersions)
            {
                var successedInfos = GetSuccessedInfos(lenstraVersion);

                Console.WriteLine(lenstraVersion + ":");
                var paddingLeft = 15;
                Console.Write("MinDivDim".PadLeft(paddingLeft));
                Console.Write("Success".PadLeft(paddingLeft));
                Console.Write("Nonsuccess".PadLeft(paddingLeft));
                Console.WriteLine();

                foreach (var successedInfo in successedInfos.OrderBy(info => info.MinDividerDigits))
                {
                    Console.Write($"{successedInfo.MinDividerDigits}".PadLeft(paddingLeft));
                    Console.Write($"{successedInfo.SuccessedCount}".PadLeft(paddingLeft));
                    Console.Write($"{successedInfo.NotSuccessedCount}".PadLeft(paddingLeft));
                    Console.WriteLine();
                }

                Console.WriteLine();
            }
        }

        private static SuccessedInfo[] GetSuccessedInfos(string lenstraVersion)
        {
            var allLenstraResults = GetAllLenstraResults(lenstraVersion);

            var successedInfos = allLenstraResults
                .GroupBy(r => r.DividerDigitsCount)
                .Select(g =>
                {
                    var lenstraResults = g.ToArray();
                    var successedCount = lenstraResults.Count(lenstraResult => lenstraResult.Success);
                    var notSuccessedCount = lenstraResults.Length - successedCount;

                    return new SuccessedInfo
                    {
                        MinDividerDigits = g.Key,
                        SuccessedCount = successedCount,
                        NotSuccessedCount = notSuccessedCount
                    };
                })
                .ToArray();

            return successedInfos;
        }

        private static List<ParsedLenstraFactorizationResult> GetAllLenstraResults(string lenstraVersion)
        {
            if (Dictionary.ContainsKey(lenstraVersion))
            {
                return Dictionary[lenstraVersion];
            }

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var dataDir = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\EllepticCurveResultGeneration\Data",
                lenstraVersion));

            var numberRegex = new Regex(@"\d+");
            var filesInfo = Directory.GetFiles(dataDir)
                .Where(fileName => fileName.EndsWith(".txt"))
                .Select(fileName =>
                {
                    var minDivStr = numberRegex.Match(fileName).Groups[0];
                    return new
                    {
                        FileName = fileName,
                        MinDividerDigits = Convert.ToInt32(minDivStr.Value)
                    };
                })
                .ToArray();

            var allLenstraResults = new List<ParsedLenstraFactorizationResult>();
            foreach (var fileInfo in filesInfo)
            {
                var lenstraResults = File.ReadAllLines(fileInfo.FileName)
                    .Select(row =>
                    {
                        var words = row.Split('|');
                        var lenstraResult = new ParsedLenstraFactorizationResult
                        {
                            TargetNumber = BigInteger.Parse(words[0]),
                            WastedTime = TimeSpan.FromTicks(Convert.ToInt64(words[2])),
                            DividerDigitsCount = fileInfo.MinDividerDigits
                        };

                        var parsedDivider = BigInteger.Parse(words[1]);
                        if (parsedDivider != BigInteger.One && parsedDivider != lenstraResult.TargetNumber)
                        {
                            lenstraResult.Divider = parsedDivider;
                        }

                        return lenstraResult;
                    })
                    .ToArray();

                allLenstraResults.AddRange(lenstraResults);
            }

            Dictionary[lenstraVersion] = allLenstraResults;
            return allLenstraResults;
        }
    }
}