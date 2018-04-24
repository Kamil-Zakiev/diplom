using System;
using System.Numerics;
using ExtraUtils;
using Utils;

namespace EdwardsCurves
{
    /// <summary> Calculator for points of a Edwards curve </summary>
    public class EdwardsCurvePointCalculator
    {
        /// <summary> Sum two points of the same curve </summary>
        /// <param name="point1">First point</param>
        /// <param name="point2">Second point</param>
        /// <returns>Sum point</returns>
        /// <exception cref="InvalidOperationException">If points belong to different curves</exception>
        public EdwardsCurvePoint Sum(EdwardsCurvePoint point1, EdwardsCurvePoint point2)
        {
            if (point1.EdwardsCurve != point2.EdwardsCurve)
            {
                throw new InvalidOperationException("It's not allowed to sum points of two different curves");
            }

            var (xSum, ySum) = CalcSum(point1, point2);
            
            return new EdwardsCurvePoint(xSum, ySum, point1.EdwardsCurve);
        }

        private (BigInteger xSum,BigInteger ySum) CalcSum(EdwardsCurvePoint point1, EdwardsCurvePoint point2)
        {
            var x1 = point1.ParameterX;
            var x2 = point2.ParameterX;

            var y1 = point1.ParameterY;
            var y2 = point2.ParameterY;

            var d = ((EdwardsCurve) point1.EdwardsCurve).ParameterD;
            var x1x2 = BigInteger.Multiply(x1, x2);
            var y1y2 = BigInteger.Multiply(y1, y2);
            var x1x2y1y2 = BigInteger.Multiply(x1x2, y1y2);
            var znam = BigInteger.Multiply(d, x1x2y1y2);

            var fieldOrder = point1.EdwardsCurve.FieldOrder;
            var znam1 = CheckInverse1(fieldOrder, znam, point1, point2).Mod(fieldOrder);
            var znam2 = CheckInverse2(fieldOrder, znam, point1, point2).Mod(fieldOrder);

            var chisl1 =  BigInteger.Multiply(x1, y2) +  BigInteger.Multiply(y1, x2);
            var chisl2 =  BigInteger.Multiply(y1, y2) -  BigInteger.Multiply(x1, x2);

            CheckZnam(znam1, fieldOrder, point1, point2);
            CheckZnam(znam2, fieldOrder, point1, point2);
            
            if (point1.ParameterX != 0 && BigInteger.GreatestCommonDivisor(point1.ParameterX, fieldOrder) > 1)
            {
                var gcd = BigInteger.GreatestCommonDivisor(point1.ParameterX, fieldOrder);
                var message = point1.ToString() + " + " + point2.ToString();
                throw new GcdFoundException(gcd, message);
            }
            if (point2.ParameterX != 0 &&BigInteger.GreatestCommonDivisor(point2.ParameterX, fieldOrder) > 1)
            {
                var gcd = BigInteger.GreatestCommonDivisor(point2.ParameterX, fieldOrder);
                var message = point1.ToString() + " + " + point2.ToString();
                throw new GcdFoundException(gcd, message);
            }

            return ( BigInteger.Multiply(chisl1, znam1),  BigInteger.Multiply(chisl2, znam2));
        }

        private static BigInteger CheckInverse1(BigInteger fieldOrder, BigInteger znam, EdwardsCurvePoint point1, EdwardsCurvePoint point2)
        {
            var modPreZnam1 = (1 + znam).Mod(fieldOrder);
            var znam1 = modPreZnam1.Inverse(fieldOrder);
            if (znam1 == 0)
            {
                var gcd = BigInteger.GreatestCommonDivisor(modPreZnam1, fieldOrder);
                var message = point1.ToString() + " + " + point2.ToString();
                throw new GcdFoundException(gcd, message);
            }

            return znam1;
        }
        
        private static BigInteger CheckInverse2(BigInteger fieldOrder, BigInteger znam, EdwardsCurvePoint point1, EdwardsCurvePoint point2)
        {
            var modPreZnam1 = (1 - znam).Mod(fieldOrder);
            var znam2 = modPreZnam1.Inverse(fieldOrder);
            if (znam2 == 0)
            {
                var gcd = BigInteger.GreatestCommonDivisor(modPreZnam1, fieldOrder);
                var message = point1.ToString() + " + " + point2.ToString();
                throw new GcdFoundException(gcd, message);
            }

            return znam2;
        }

        private void CheckZnam(BigInteger znam1, BigInteger fieldOrder, EdwardsCurvePoint point1, EdwardsCurvePoint point2)
        {
            if (znam1 != 0 && BigInteger.GreatestCommonDivisor(znam1, fieldOrder) > 1)
            {
                var gcd = BigInteger.GreatestCommonDivisor(znam1, fieldOrder);
                var message = point1.ToString() + " + " + point2.ToString();
                throw new GcdFoundException(gcd, message);
            }
        }

        public EdwardsCurvePoint Mult(BigInteger k, EdwardsCurvePoint p)
        {
            if (k.IsZero)
            {
                throw new Exception("Попытка умножить на 0");
            }

            var b = p;
            var q = ((EdwardsCurve)p.EdwardsCurve).NeitralPoint;

            while (!k.IsZero)
            {
                if (k % 2 == 1)
                {
                    q = Sum(q, b);
                }
                b = Sum(b, b);
                k /= 2;
            }
            return q;
        }
    }
}