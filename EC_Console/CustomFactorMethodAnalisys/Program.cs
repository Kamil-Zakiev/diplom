using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using EdwardsCurves.ProjectiveEdwardsCurves.Lenstra;
using LenstraAlgorithm;

namespace CustomFactorMethodAnalisys
{
    /// <summary>
    /// Анализ результатов среднего времени работы реализованного алгоритма быстрой факторизации на основе ЭК
    /// </summary>
    internal class Program
    {
        
        public static string SemiprimesResourceDir = @"E:\Stash\diplom\EC_Console\SemiPrimeNumbersGenerator\Resources";
        
        public static void Main(string[] args)
        {
            /*
             Суть реализованного алгоритма заключается в следующем: 
             выбирается количество эллиптических кривых, равное количеству процессоров на машине, 
             и по этим кривым параллельно запускается варианты алгоритма Ленстры. 
             Как только один из вариантов алгоритма закончил работу (неважно, успешно или неуспешно), 
             остальные варианты алгоритма прекращают своё выполнение. 
             */

            const int semiprimesPerDim = 10; 
            const int startDim = 4;
            const int endDim = 7;
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
                .ToList();

            var numbersPackages = semiprimesFiles.Select(fileInfo => new
                {
                    Numbers = File.ReadAllLines(fileInfo.FileName).Take(semiprimesPerDim).Select(BigInteger.Parse)
                        .ToArray(),
                    MinDividerSize = fileInfo.MinDividerDigits
                })
                .ToArray();
            
            Console.Write("dim\tfast\tstd");
            foreach (var numbersPackage in numbersPackages)
            {
                var numbers = numbersPackage.Numbers;
                var fastTimes = numbers.Select(GetMsOfFastAlg).Where(x => x.success).Select(x => x.ms).ToArray();
                var stndTimes = numbers.Select(GetMsOfStandartAlg).Where(x => x.success).Select(x => x.ms).ToArray();
                    
                Console.Write(numbersPackage.MinDividerSize + "\t");
                Console.Write($"{fastTimes.Average():F2}\t{stndTimes.Average():F2}\n");
            }
        }

        private static (bool success, double ms) GetMsOfFastAlg(BigInteger n)
        {
            var multithreadLenstra = new MultithreadLenstra<ProjectiveEdwardsLenstra>();
            
            var stopWatch = new Stopwatch();
            var divider = multithreadLenstra.LenstraMultiThreadFastResult(n, Environment.ProcessorCount);
            stopWatch.Stop();

            return (divider.HasValue, stopWatch.ElapsedMilliseconds);
        }

        private static (bool success, double ms) GetMsOfStandartAlg(BigInteger n)
        {
            var projectiveEdwardsLenstra = new ProjectiveEdwardsLenstra();
            var stopWatch = new Stopwatch();
            var result = projectiveEdwardsLenstra.GetDivider(n, new Random());
            stopWatch.Stop();

            return (result.Success, stopWatch.ElapsedMilliseconds);
        }
    }
}