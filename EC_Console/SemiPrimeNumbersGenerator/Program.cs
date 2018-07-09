namespace SemiPrimeNumbersGenerator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using Utils;

    /// <summary> Генератор полупростых чисел </summary>
    internal class Program
    {
        public const int NumbersCountForDimension = 1000;

        public static Random Random = new Random();

        public static void Main(string[] args)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var resourcesDir = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\Resources"));

            var dimensions = Enumerable.Range(4, 17).ToArray();
            Parallel.ForEach(dimensions, (dividerDim) =>
            {
                var fileName = $"MinDivider{dividerDim}Digit.txt";
                var path = Path.Combine(resourcesDir, fileName);
                GenerateTwoPrimesMultipleNumbersInFile(path, dividerDim);
                Console.WriteLine($"{fileName} has been generated");
            });
        }

        public static void GenerateTwoPrimesMultipleNumbersInFile(string path, int dividerSize)
        {
            var topBound = BigInteger.Pow(10, dividerSize);
            var bottonBound = BigInteger.Pow(10, dividerSize - 1);

            BigInteger GetSizedRandomPrime()
            {
                BigInteger number = 0;
                while (number >= topBound || number <= bottonBound)
                {
                    var randomNumber = BigIntegerExtensions.GetNextRandom(Random, BigInteger.Pow(10, dividerSize));
                    number = BigIntegerExtensions.NextPrimaryMillerRabin(randomNumber);
                }

                return number;
            }

            var semiPrimeNumbers = new List<string>();
            for (var i = 0; i < NumbersCountForDimension; i++)
            {
                var first = GetSizedRandomPrime();
                var second = GetSizedRandomPrime();
                var semiPrime = first * second;
                semiPrimeNumbers.Add(semiPrime.ToString());
            }

            File.WriteAllLines(path, semiPrimeNumbers);
        }
    }
}