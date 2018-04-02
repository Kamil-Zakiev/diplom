using System;
using System.Numerics;

namespace EdwardsCurves
{
    /// <summary> Calculator for points of a projective Edwards curve </summary>
    public class ProjectiveEdwardsCurvePointCalculator
    {
        /// <summary> Sum two points of the same curve </summary>
        /// <param name="point1">First point</param>
        /// <param name="point2">Second point</param>
        /// <returns>Sum point</returns>
        /// <exception cref="InvalidOperationException">If points belong to different curves</exception>
        public ProjectiveEdwardsCurvePoint Sum(ProjectiveEdwardsCurvePoint point1, ProjectiveEdwardsCurvePoint point2)
        {
            if (point1.ProjectiveEdwardsCurve != point2.ProjectiveEdwardsCurve)
            {
               // throw new InvalidOperationException("It's not allowed to sum points of two different curves");
            }
            

            var x1 = point1.ParameterX;
            var x2 = point2.ParameterX;
            var y1 = point1.ParameterY;
            var y2 = point2.ParameterY;
            var z1 = point1.ParameterZ;
            var z2 = point2.ParameterZ;
            var parameterD = point1.ProjectiveEdwardsCurve.ParameterD;

            var a = z1 * z2;
            var b = a * a;
            var c = x1 * x2;
            var d = y1 * y2;
            var e = parameterD * c * d;
            var f = b - e;
            var g = b + e;

            var xSum = a * f * ((x1 + y1) * (x2 + y2) - c - d);
            var ySum = a * g * (d - c);
            var zSum = c * f * g;
            
            return new ProjectiveEdwardsCurvePoint(xSum, ySum, zSum, point1.ProjectiveEdwardsCurve);
        }
        
//        public ProjectiveEdwardsCurvePoint Mult(BigInteger k, ProjectiveEdwardsCurvePoint p)
//        {
//            if (k.IsZero)
//                throw new Exception("Попытка умножить на 0");
//
//            ProjectiveEdwardsCurvePoint b = p;
//            ProjectiveEdwardsCurvePoint q = new ProjectiveEdwardsCurvePoint(p.ProjectiveEdwardsCurve)
//            {
//                //IsInfinity = true
//            };
//
//            while (!k.IsZero)
//            {
//                if (k % 2 == 1)
//                    q = Sum(q, b);
//                b = Sum(b, b);
//                k /= 2;
//            }
//            return q;
//        }
    }
}