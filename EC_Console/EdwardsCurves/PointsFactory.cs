using System;
using System.Numerics;

namespace EdwardsCurves
{
    public class PointsFactory
    {
        public ProjectiveEdwardsCurve ProjectiveEdwardsCurve { get; }

        public PointsFactory(ProjectiveEdwardsCurve projectiveEdwardsCurve)
        {
            ProjectiveEdwardsCurve = projectiveEdwardsCurve;
        }

        public ProjectiveEdwardsCurvePoint CreatePoint(BigInteger parameterX, BigInteger parameterY,
            BigInteger parameterZ)
        {
            CheckPointOnCurve(parameterX, parameterY, parameterZ);

            return new ProjectiveEdwardsCurvePoint(parameterX, parameterY, parameterZ, ProjectiveEdwardsCurve);
        }

        private void CheckPointOnCurve(BigInteger x, BigInteger y, BigInteger z)
        {
            var x2 = x * x;
            var y2 = y * y;
            var z2 = z * z;
            var left = x2 * z2 + y2 * z2;
            var right = z2 * z2 + ProjectiveEdwardsCurve.ParameterD * x2 * y2;

            var diff = left - right;
            if (diff % ProjectiveEdwardsCurve.FieldOrder == 0)
            {
                return;
            }
            
            throw new InvalidOperationException("Точка не принадлежит кривой");
        }
    }
}