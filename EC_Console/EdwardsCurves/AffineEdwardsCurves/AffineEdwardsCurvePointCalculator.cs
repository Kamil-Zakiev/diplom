namespace EdwardsCurves.AffineEdwardsCurves
{
    using System;
    using System.Numerics;
    using Utils;

    /// <summary> Calculator for points of a Edwards curve </summary>
    public class AffineEdwardsCurvePointCalculator
    {
        /// <summary> Sum two points of the same curve </summary>
        /// <param name="point1">First point</param>
        /// <param name="point2">Second point</param>
        /// <returns>Sum point</returns>
        /// <exception cref="InvalidOperationException">If points belong to different curves</exception>
        public AffineEdwardsCurvePoint Sum(AffineEdwardsCurvePoint point1, AffineEdwardsCurvePoint point2)
        {
            if (point1.EdwardsCurve != point2.EdwardsCurve)
            {
                throw new InvalidOperationException("It's not allowed to sum points of two different curves");
            }

            var (xSum, ySum) = CalcSum(point1, point2);

            return new AffineEdwardsCurvePoint(xSum, ySum, point1.EdwardsCurve);
        }

        private (BigInteger xSum, BigInteger ySum) CalcSum(AffineEdwardsCurvePoint point1,
            AffineEdwardsCurvePoint point2)
        {
            var x1 = point1.ParameterX;
            var x2 = point2.ParameterX;

            var y1 = point1.ParameterY;
            var y2 = point2.ParameterY;

            var d = point1.EdwardsCurve.ParameterD;
            var znam = d * x1 * x2 * y1 * y2;

            var fieldOrder = point1.EdwardsCurve.FieldOrder;
            var znam1 = (1 + znam).Mod(fieldOrder).Inverse(fieldOrder).Mod(fieldOrder);
            var znam2 = (1 - znam).Mod(fieldOrder).Inverse(fieldOrder).Mod(fieldOrder);

            var chisl1 = x1 * y2 + y1 * x2;
            var chisl2 = y1 * y2 - x1 * x2;

            var xSum = chisl1 * znam1;
            return (xSum, chisl2 * znam2);
        }

        public AffineEdwardsCurvePoint Mult(BigInteger k, AffineEdwardsCurvePoint p)
        {
            if (k.IsZero)
            {
                throw new Exception("Попытка умножить на 0");
            }

            var b = p;
            var q = ((AffineEdwardsCurve) p.EdwardsCurve).NeitralPoint;

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