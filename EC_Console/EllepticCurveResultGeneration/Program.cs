namespace EllepticCurveResultGeneration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using EC_Console;
    using EdwardsCurves.AffineEdwardsCurves.Lenstra;
    using EdwardsCurves.ProjectiveEdwardsCurves.Lenstra;
    using LenstraAlgorithm;

    /// <summary> Генерация данных для анализа результатов работы алгоритма Ленстры </summary>
    internal class Program
    {
        private const int CountForDimension = 100;
        
        private const int CurvesCountForSingleNumber = 100;
        
        private const string ContinuationInfoStorePath = @"E:\Stash\diplom\EC_Console\EllepticCurveResultGeneration\ContinuationInfoStore.txt";

        public static string SemiprimesResourceDir = @"E:\Stash\diplom\EC_Console\SemiPrimeNumbersGenerator\Resources";

        private static readonly ContinuationInfo ContinuationInfo;

        static Program()
        {
            try
            {
                ContinuationInfo = RestoreContinuationInfo();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void Main(string[] args)
        {
            const int startDim = 4;
            const int endDim = 10;
            
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var dataDir = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\Data"));
            var numberRegex = new Regex(@"\d+");
            var semiprimesFiles = Directory.GetFiles(SemiprimesResourceDir)
                .Select(fileName =>
                {
                    var minDivStr = numberRegex.Match(fileName).Groups[0];
                    return new
                    {
                        FileName = fileName,
                        MinDividerDigits = Convert.ToInt32(minDivStr.Value)
                    };
                })
                .Where(dto => startDim <= dto.MinDividerDigits && dto.MinDividerDigits <= endDim)
                .OrderBy(dto => dto.MinDividerDigits)
                .Select(dto => dto.FileName)
                .ToList();

            var lenstraVersions = new[]
                {nameof(ClassicLenstra), nameof(AffineEdwardsLenstra), nameof(ProjectiveEdwardsLenstra)};

            if (!string.IsNullOrWhiteSpace(ContinuationInfo.LenstraVersion))
            {
                lenstraVersions = lenstraVersions.SkipWhile(lv => lv != ContinuationInfo.LenstraVersion).ToArray();
            }
            
            foreach (var lenstraVersion in lenstraVersions)
            {
                if (lenstraVersion == nameof(ClassicLenstra))
                {
                    ProcessFiles<ClassicLenstra>(semiprimesFiles, dataDir, lenstraVersion);
                }
                else if (lenstraVersion == nameof(AffineEdwardsLenstra))
                {
                    ProcessFiles<AffineEdwardsLenstra>(semiprimesFiles, dataDir, lenstraVersion);
                }
                else if (lenstraVersion == nameof(ProjectiveEdwardsLenstra))
                {
                    ProcessFiles<ProjectiveEdwardsLenstra>(semiprimesFiles, dataDir, lenstraVersion);
                }
            }
        }

        private static ContinuationInfo RestoreContinuationInfo()
        {
            var lines = File.ReadAllLines(ContinuationInfoStorePath);
            if (!lines.Any() || !lines[0].Contains("|"))
            {
                return new ContinuationInfo();
            }

            var words = lines[0].Split('|');
            var result = new ContinuationInfo()
            {
                LenstraVersion = words[0],
                ProcessingFileName = words[1]
            };
            
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var dataDir = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\Data", result.LenstraVersion));
            var lastCreatedFile = Directory.GetFiles(dataDir)
                .Where(fileName => fileName.EndsWith(".txt"))
                .OrderBy(File.GetCreationTime)
                .LastOrDefault();

            if (lastCreatedFile == null)
            {
                return result;
            }

            var lastHandledNumber = File.ReadAllLines(lastCreatedFile).Last().Split('|')[0];
            var lastProcessedLine = File.ReadAllLines(result.ProcessingFileName)
                                        .Select(line => line.Split('|')[0])
                                        .TakeWhile(tn => tn != lastHandledNumber)
                                        .Count()
                                    + 1;

            result.LastProcessedLine = lastProcessedLine;

            return result;
        }

        private static void ProcessFiles<TLenstra>(List<string> semiprimesFiles, string dataDir, string version) where TLenstra:ILenstra, new()
        {
            var mtl = new MultithreadLenstra<TLenstra>();
            
            if (version == ContinuationInfo.LenstraVersion && !string.IsNullOrWhiteSpace(ContinuationInfo.ProcessingFileName))
            {
                semiprimesFiles = semiprimesFiles
                    .SkipWhile(fileName => fileName != ContinuationInfo.ProcessingFileName)
                    .ToList();
            }

            var lenstraVersion = typeof(TLenstra).Name;
            foreach (var semiprimesFile in semiprimesFiles)
            {
                File.WriteAllText(ContinuationInfoStorePath, lenstraVersion + "|" + semiprimesFile);
                var fileName = Path.GetFileName(semiprimesFile);

                var path = Path.Combine(dataDir, lenstraVersion, fileName ?? throw new FileNotFoundException());
                
                var semiprimes = File.ReadAllLines(semiprimesFile).Take(CountForDimension).Select(BigInteger.Parse)
                    .ToArray();
                
                if (version == ContinuationInfo.LenstraVersion && ContinuationInfo.ProcessingFileName == semiprimesFile)
                {
                    semiprimes = semiprimes.Skip(ContinuationInfo.LastProcessedLine).ToArray();
                }

                double k = 0;
                foreach (var semiprime in semiprimes)
                {
                    Console.Write("\rHandling " + semiprime + ". Processed " + (k / semiprimes.Length).ToString("P") + " of " + fileName);
                    k++;
                    var results = mtl.LenstraMultiThreadResults(semiprime, CurvesCountForSingleNumber);
                    var infos =  results
                            .Select(result => $"{result.TargetNumber}|{result.Divider}|{result.WastedTime.Ticks}")
                            .ToArray();
                    File.AppendAllLines(path, infos);
                }
            }
        }
    }

    /// <summary> Информация для продолжения после прерывания </summary>
    internal class ContinuationInfo
    {
        public string LenstraVersion { get; set; }
        
        public string ProcessingFileName { get; set; }

        public int LastProcessedLine { get; set; }
    }
}