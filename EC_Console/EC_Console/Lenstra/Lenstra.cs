﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace EC_Console
{
    public class Lenstra
    {
        public BigInteger B1 = BigInteger.Parse("100000");

        /// <summary>  Возвращает объект LenstraResultOfEllepticCurve </summary>
        /// <param name="n">Число, у которого требуется найти делитель</param>
        public LenstraResultOfEllepticCurve GetDivider(BigInteger n, Random random, EllepticCurve ellepricCurve = null)
        {
            var startTime = DateTime.Now;
            BigInteger g, x, y, a, b;
            int k = 0;
            do
            {
                x = BigIntegerExtensions.GetNextRandom(random, n);
                y = BigIntegerExtensions.GetNextRandom(random, n);
                a = BigIntegerExtensions.GetNextRandom(random, n);
                k++;

                b = BigIntegerExtensions.Mod(y * y - x * x * x - a * x, n);
                g = BigInteger.GreatestCommonDivisor(n, 4 * a * a * a + 27 * b * b);
            } while (g == n);

            //убираем влияние выбора рандомного числа на время работы алгоритма
            startTime = startTime + new TimeSpan(0, 0, 0, k*3);

            EllepticCurve ec = null;
            try
            {
                if (g != 1)
                    throw new GCDFoundException(g);
                ec = new EllepticCurve(a, b, n);
                var p0 = new PointOfEC
                {
                    EllepticCurve = ec,
                    X = x,
                    Y = y
                };
                ec.Basis = p0;

                var P = new PointOfEC(p0);

                BigInteger p = 2;
                while (p < B1)
                {
                    var pr = p;
                    while (pr < B1)
                    {
                        P = ec.Mult(p, P);
                        pr *= p;
                    }
                    p = BigIntegerExtensions.NextPrimaryMillerRabin(p);
                }
            }
            catch (GCDFoundException exc)
            {
                Console.WriteLine("Поток {0} молодец: {1} = {2} * {3}", 
                    Task.CurrentId, n, exc.GreatestCommonDivisor, n / exc.GreatestCommonDivisor);

                return new LenstraResultOfEllepticCurve
                {
                    EllepticCurve = ec,
                    TargetNumber = n,
                    Divider = exc.GreatestCommonDivisor,
                    WastedTime = DateTime.Now - startTime
                };
            }

            return new LenstraResultOfEllepticCurve
            {
                EllepticCurve = ec,
                TargetNumber = n,
                WastedTime = DateTime.Now - startTime
            };
        }
    }
}