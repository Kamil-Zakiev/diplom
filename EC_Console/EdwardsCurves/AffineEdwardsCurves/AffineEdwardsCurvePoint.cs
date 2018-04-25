namespace EdwardsCurves.AffineEdwardsCurves
{
    using System.Numerics;
    using ProjectiveEdwardsCurves;
    using Utils;

    /// <summary> Точка на кривой Эдвардса в аффинных координатах </summary>
    public class AffineEdwardsCurvePoint
    {
        public AffineEdwardsCurvePoint(BigInteger x, BigInteger y, IEdwardsCurve edwardsCurve)
        {
            ParameterX = x.Mod(edwardsCurve.FieldOrder);
            ParameterY = y.Mod(edwardsCurve.FieldOrder);

            EdwardsCurve = edwardsCurve;
        }

        /// <summary> Параметр x </summary>
        public BigInteger ParameterX { get; }

        /// <summary> Параметр y </summary>
        public BigInteger ParameterY { get; }

        /// <summary> Кривая, которой принадлежит эта точка </summary>
        public IEdwardsCurve EdwardsCurve { get; }

        public ProjectiveEdwardsCurvePoint ToProjectiveEdwardsCurvePoint(BigInteger? z = null)
        {
            if (!z.HasValue)
            {
                z = BigInteger.One;
            }

            var x = ParameterX * z.Value;
            var y = ParameterY * z.Value;
            var z2 = z.Value;

            return new ProjectiveEdwardsCurvePoint(x, y, z2, EdwardsCurve);
        }

        public override string ToString()
        {
            return $"(x = {ParameterX}, y = {ParameterY})";
        }
    }
}