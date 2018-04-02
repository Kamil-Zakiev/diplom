using System.Numerics;

namespace ExtraUtils
{
    public static class ExtraBigIntegerExtensions
    {
        public static BigInteger Mod(this BigInteger a, BigInteger p)
        {
            if (a.Sign >= 0)
                return a % p;
            return a % p + p;
        }
        
        /// <summary> Найти обратный элемент к "а" в поле размерности "p" </summary>
        /// <param name="a">Число</param>
        /// <param name="p">Размерность поля</param>
        public static BigInteger Inverse(this BigInteger a, BigInteger p)
        {
            if (a < 0)
            {
                a = a % p + p;
            }
            if (a > p)
            {
                a = a % p;
            }
            BigInteger d, x, y;
            ExtendedEuclid(a, p, out x, out y, out d);

            if (d == BigInteger.One) return x;

            return 0;
        }
        
        /// <summary>
        /// Расширенный алгоритм Евклида
        /// ax+by=GCA(a,b)
        /// </summary>
        private static void ExtendedEuclid(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y, out BigInteger d)
        {
            BigInteger q, r, x1, x2, y1, y2;

            if (b == 0)
            {
                d = a; x = 1; y = 0;
                return;
            }

            x2 = 1; x1 = 0; y2 = 0; y1 = 1;

            while (b > 0)
            {
                q = a / b; r = a - q * b;
                x = x2 - q * x1; y = y2 - q * y1;

                a = b;
                b = r;

                x2 = x1;
                x1 = x;
                y2 = y1;
                y1 = y;
            }

            d = a;
            x = x2; y = y2;
        }

    }
}