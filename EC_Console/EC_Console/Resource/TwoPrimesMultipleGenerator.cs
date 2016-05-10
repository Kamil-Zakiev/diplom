using System.IO;
using System.Linq;
using System.Numerics;

namespace EC_Console
{
    public class TwoPrimesMultipleGenerator
    {
        public static void GenerateTwoPrimesMultipleNumbersInFile(string path, int dividerSize)
        {
            var start = BigIntegerExtensions.NextPrimaryMillerRabin(BigInteger.Pow(10, dividerSize));
            //генерим 100 чисел
            var twoPrimesMultipleNumbers = new BigInteger[100];
            for (int i = 0; i < 100; i++)
            {
                var next = BigIntegerExtensions.NextPrimaryMillerRabin(start);
                twoPrimesMultipleNumbers[i] = start*next;
                start = next;
            }
            File.WriteAllLines(path, twoPrimesMultipleNumbers.Select(x => x.ToString()));
        }
    }
}
