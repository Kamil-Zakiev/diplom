using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using EC_Console;
using EdwardsCurves.AffineEdwardsCurves.Lenstra;
using EdwardsCurves.ProjectiveEdwardsCurves.Lenstra;
using LenstraAlgorithm.Dto;

namespace AnalizingEllepticCurveResult
{
    /// <summary> Анализ результаты работы каждой ЭК </summary>
    internal class Program
    {
        public static void Main(string[] args)
        {
            var lenstraVersions = new[]
                {nameof(ClassicLenstra), nameof(AffineEdwardsLenstra), nameof(ProjectiveEdwardsLenstra)};

            foreach (var lenstraVersion in lenstraVersions)
            {
                var successedInfos = GetSuccessedInfos(lenstraVersion);

                Console.WriteLine(lenstraVersion + ":");
                Console.WriteLine("MinDivDim\tSuccess\tNonsuccess");
                foreach (var successedInfo in successedInfos)
                    Console.WriteLine(
                        $"{successedInfo.MinDividerDigits}\t{successedInfo.SuccessedCount}\t{successedInfo.NotSuccessedCount}");

                Console.WriteLine();
            }
        }

        private static LinkedList<SuccessedInfo> GetSuccessedInfos(string lenstraVersion)
        {
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

            var successedInfos = new LinkedList<SuccessedInfo>();
            foreach (var fileInfo in filesInfo)
            {
                var lenstraResults = File.ReadAllLines(fileInfo.FileName)
                    .Select(row =>
                    {
                        var words = row.Split('|');
                        var lenstraResult = new LenstraFactorizationResult
                        {
                            TargetNumber = BigInteger.Parse(words[0]),
                            Divider = BigInteger.Parse(words[1]),
                            WastedTime = TimeSpan.FromTicks(Convert.ToInt64(words[2]))
                        };

                        return lenstraResult;
                    })
                    .ToArray();

                var successedCount = lenstraResults.Count(lenstraResult => lenstraResult.Success);
                var notSuccessedCount = lenstraResults.Length - successedCount;

                successedInfos.AddLast(new SuccessedInfo
                {
                    MinDividerDigits = fileInfo.MinDividerDigits,
                    SuccessedCount = successedCount,
                    NotSuccessedCount = notSuccessedCount
                });
            }

            return successedInfos;
        }
    }
}