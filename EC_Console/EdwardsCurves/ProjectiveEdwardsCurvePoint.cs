using System.Numerics;
using ExtraUtils;

namespace EdwardsCurves
{
    public class ProjectiveEdwardsCurvePoint
    {
        public BigInteger ParameterX { get; }

        public BigInteger ParameterY { get; }

        public BigInteger ParameterZ { get; }
        
        public ProjectiveEdwardsCurvePoint(BigInteger parameterX, BigInteger parameterY, BigInteger parameterZ,
            ProjectiveEdwardsCurve projectiveEdwardsCurve)
        {
            ProjectiveEdwardsCurve = projectiveEdwardsCurve;

            ParameterX = parameterX.Mod(projectiveEdwardsCurve.FieldOrder);
            ParameterY = parameterY.Mod(projectiveEdwardsCurve.FieldOrder);
            ParameterZ = parameterZ.Mod(projectiveEdwardsCurve.FieldOrder);
        }

        public ProjectiveEdwardsCurve ProjectiveEdwardsCurve { get; }

        public EdwardsCurvePoint ToEdwardsCurvePoint()
        {
            var inverseZ = ParameterZ.Inverse(ProjectiveEdwardsCurve.FieldOrder);

            var x = ParameterX * inverseZ;
            var y = ParameterY * inverseZ;
            return new EdwardsCurvePoint(x, y, ProjectiveEdwardsCurve);
        }

        public override string ToString()
        {
            return $"(x = {ParameterX}, y = {ParameterY}, z = {ParameterZ})";
        }
    }
}