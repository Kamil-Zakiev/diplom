using System;
using System.Numerics;
using System.Threading;

namespace EC_Console
{
    /// <summary> Методы, дополняющие BigInteger </summary>
    public static class BigIntegerExtensions
    {
        /// <summary> Число "а" по модулю "р" </summary>
        public static BigInteger Mod(BigInteger a, BigInteger p)
        {
            if (a.Sign > 0)
                return a%p;
            return a%p + p;
        }

        /// <summary> Найти обратный элемент к "а" в поле размерности "p" </summary>
        /// <param name="a">Число</param>
        /// <param name="p">Размерность поля</param>
        public static BigInteger Inverse(BigInteger a, BigInteger p)
        {
            if (a < 0)
            {
                a = a%p + p;
            }
            if (a > p)
            {
                a = a%p;
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

        /// <summary>
        /// Возвращает случайное число меньшее 'n'
        /// </summary>
        /// <param name="random"> Объект - генератор случайных чисел </param>
        public static BigInteger GetNextRandom(Random random, BigInteger n)
        {
            Thread.Sleep(1000);
            var bytesN = n.ToByteArray();
            var bytesa = new byte[bytesN.Length];
            random.NextBytes(bytesa);
            var a = BigInteger.Abs(new BigInteger(bytesa));
            if (a >= n)
                return Mod(a, n);
            return a;
        }

        /// <summary> 
        /// Возвращает случайное число меньшее 'N'. 
        /// Используется в проверке простоты Миллер-Рабина
        /// </summary>
        private static BigInteger RandomIntegerBelow(BigInteger N)
        {
            var random = new Random();
            byte[] bytes = N.ToByteArray();
            BigInteger R;

            do
            {
                random.NextBytes(bytes);
                bytes[bytes.Length - 1] &= (byte)0x7F; //force sign bit to positive
                R = new BigInteger(bytes);
            } while (R >= N);

            return R;
        }
        /// <summary>
        /// Проверка простоты Миллера-Рабина. 
        /// По умолчанию проводится 100 раундов
        /// </summary>
        private static bool IsPrimaryMillerRabin(BigInteger n, int k = 100)
        {
            if (n <= 1)
                return false;
            if (n == 2)
                return true;
            if (n % 2 == 0)
                return false;

            BigInteger s = 0, d = n - 1;
            while (d % 2 == 0)
            {
                d /= 2;
                s++;
            }
            for (int i = 0; i < k; i++)
            {
                BigInteger a = RandomIntegerBelow(n - 1);
                BigInteger x = BigInteger.ModPow(a, d, n);
                if (x == 1 || x == n - 1)
                    continue;
                for (int j = 0; j < s - 1; j++)
                {
                    x = (x * x) % n;
                    if (x == 1)
                        return false;
                    if (x == n - 1)
                        break;
                }
                if (x != n - 1)
                    return false;
            }
            return true;
        }

        /// <summary> Возвращает следующее простое число после R </summary>
        public static BigInteger NextPrimaryMillerRabin(BigInteger R)
        {
            R++;
            if (IsPrimaryMillerRabin(R))
                return R;

            if (R % 2 == 0)
                R++;

            while (!IsPrimaryMillerRabin(R))
                R += 2;

            return R;
        }
    }
}
