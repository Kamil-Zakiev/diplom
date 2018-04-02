using System.Numerics;
using ExtraUtils;

namespace EdwardsCurves
{
    public class EdwardsCurvePoint
    {
        public BigInteger ParameterX { get; }
        
        public BigInteger ParameterY { get; }

        public EdwardsCurvePoint(BigInteger x, BigInteger y, ProjectiveEdwardsCurve projectiveEdwardsCurve)
        {
            ParameterX = x.Mod(projectiveEdwardsCurve.FieldOrder);
            ParameterY = y.Mod(projectiveEdwardsCurve.FieldOrder);

            ProjectiveEdwardsCurve = projectiveEdwardsCurve;
        }
        
        public ProjectiveEdwardsCurve ProjectiveEdwardsCurve { get; }

        public ProjectiveEdwardsCurvePoint ToProjectiveEdwardsCurvePoint(BigInteger? z = null)
        {
            if (!z.HasValue)
            {
                z = BigInteger.One;
            }

            var x = ParameterX * z.Value;
            var y = ParameterY * z.Value;
            var z2 = z.Value;

            return new ProjectiveEdwardsCurvePoint(x, y, z2, ProjectiveEdwardsCurve);
        }
        
        public override string ToString()
        {
            return $"(x = {ParameterX}, y = {ParameterY})";
        }
    }
}