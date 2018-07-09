namespace EdwardsCurves.ProjectiveEdwardsCurves
{
    using System.Numerics;

    /// <summary> Кривая Эдвардса в проективных координатах </summary>
    /// <inheritdoc cref="IEdwardsCurve" />
    public class ProjectiveEdwardsCurve : IEdwardsCurve
    {
        /// <summary> Нейтральная по суммированию точка кривой Эдвардса </summary>
        public readonly ProjectiveEdwardsCurvePoint NeitralPoint;

        public ProjectiveEdwardsCurve(BigInteger parameterD, BigInteger fieldOrder)
        {
            ParameterD = parameterD;
            FieldOrder = fieldOrder;

            NeitralPoint = new ProjectiveEdwardsCurvePoint(0, 1, 1, this);
        }

        public BigInteger ParameterD { get; }

        public BigInteger FieldOrder { get; }
    }
}