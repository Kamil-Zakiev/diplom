namespace EdwardsCurves.ProjectiveEdwardsCurves
{
    using System.Numerics;
    using AffineEdwardsCurves;
    using Utils;

    /// <summary> Точка на кривой Эдвардса в проективных координатах </summary>
    public class ProjectiveEdwardsCurvePoint
    {
        public ProjectiveEdwardsCurvePoint(BigInteger parameterX, BigInteger parameterY, BigInteger parameterZ,
            IEdwardsCurve projectiveEdwardsCurve)
        {
            ProjectiveEdwardsCurve = projectiveEdwardsCurve;

            ParameterX = parameterX.Mod(projectiveEdwardsCurve.FieldOrder);
            ParameterY = parameterY.Mod(projectiveEdwardsCurve.FieldOrder);
            ParameterZ = parameterZ.Mod(projectiveEdwardsCurve.FieldOrder);
        }

        /// <summary> Параметр x </summary>
        public BigInteger ParameterX { get; }

        /// <summary> Параметр y </summary>
        public BigInteger ParameterY { get; }

        /// <summary> Параметр z </summary>
        public BigInteger ParameterZ { get; }

        /// <summary> Кривая, которой принадлежит точка </summary>
        public IEdwardsCurve ProjectiveEdwardsCurve { get; }

        public AffineEdwardsCurvePoint ToEdwardsCurvePoint()
        {
            var inverseZ = ParameterZ.Inverse(ProjectiveEdwardsCurve.FieldOrder);

            var x = ParameterX * inverseZ;
            var y = ParameterY * inverseZ;
            return new AffineEdwardsCurvePoint(x, y, ProjectiveEdwardsCurve);
        }

        public override string ToString()
        {
            return $"(x = {ParameterX}, y = {ParameterY}, z = {ParameterZ})";
        }

        public override bool Equals(object obj)
        {
            var otherPoint = (ProjectiveEdwardsCurvePoint) obj;

            // потому что этот метод нужен лишь для тестов. Производительность не важна
            return ToEdwardsCurvePoint().ToString() == otherPoint.ToEdwardsCurvePoint().ToString();
        }
    }
}