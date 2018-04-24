using System.Numerics;
using ExtraUtils;
using LenstraAlgorithm;

namespace EdwardsCurves
{
    public class EdwardsCurvePoint
    {
        public BigInteger ParameterX { get; }
        
        public BigInteger ParameterY { get; }

        public EdwardsCurvePoint(BigInteger x, BigInteger y, IEdwardsCurve edwardsCurve)
        {
            ParameterX = x.Mod(edwardsCurve.FieldOrder);
            ParameterY = y.Mod(edwardsCurve.FieldOrder);

            EdwardsCurve = edwardsCurve;
        }
        
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