namespace EdwardsCurves
{
    using System;
    using System.Numerics;
    using AffineEdwardsCurves;
    using ProjectiveEdwardsCurves;

    /// <summary> Фабрика точек, создающая точки на кривой с проверкой принадлежности точки к кривой </summary>
    public class PointsFactory
    {
        public PointsFactory(IEdwardsCurve projectiveEdwardsCurve)
        {
            EdwardsCurve = projectiveEdwardsCurve;
        }

        public IEdwardsCurve EdwardsCurve { get; }

        public ProjectiveEdwardsCurvePoint CreatePoint(BigInteger parameterX, BigInteger parameterY,
            BigInteger parameterZ)
        {
            CheckPointOnCurve(parameterX, parameterY, parameterZ);

            return new ProjectiveEdwardsCurvePoint(parameterX, parameterY, parameterZ, EdwardsCurve);
        }

        public AffineEdwardsCurvePoint CreatePoint(BigInteger parameterX, BigInteger parameterY)
        {
            CheckPointOnCurve(parameterX, parameterY);

            return new AffineEdwardsCurvePoint(parameterX, parameterY, EdwardsCurve);
        }

        private void CheckPointOnCurve(BigInteger x, BigInteger y)
        {
            var x2 = x * x;
            var y2 = y * y;
            var left = x2 + y2;
            var right = 1 + EdwardsCurve.ParameterD * x2 * y2;

            var diff = left - right;
            if (diff % EdwardsCurve.FieldOrder == 0)
            {
                return;
            }

            throw new InvalidOperationException("Точка не принадлежит кривой");
        }

        public bool SoftCheckPointOnCurve(BigInteger x, BigInteger y)
        {
            var x2 = x * x;
            var y2 = y * y;
            var left = x2 + y2;
            var right = 1 + EdwardsCurve.ParameterD * x2 * y2;

            var diff = left - right;
            return diff % EdwardsCurve.FieldOrder == 0;
        }

        private void CheckPointOnCurve(BigInteger x, BigInteger y, BigInteger z)
        {
            var x2 = x * x;
            var y2 = y * y;
            var z2 = z * z;
            var left = x2 * z2 + y2 * z2;
            var right = z2 * z2 + EdwardsCurve.ParameterD * x2 * y2;

            var diff = left - right;
            if (diff % EdwardsCurve.FieldOrder == 0)
            {
                return;
            }

            throw new InvalidOperationException("Точка не принадлежит кривой");
        }
    }
}