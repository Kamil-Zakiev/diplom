namespace EdwardsCurves.AffineEdwardsCurves
{
    using System.Numerics;

    /// <summary> Кривая Эдвардса в аффинных координатах </summary>
    /// <inheritdoc cref="IEdwardsCurve" />
    /// >
    public class AffineEdwardsCurve : IEdwardsCurve
    {
        /// <summary> Нейтральный по суммированию элемент </summary>
        public readonly AffineEdwardsCurvePoint NeitralPoint;

        public AffineEdwardsCurve(BigInteger parameterD, BigInteger fieldOrder)
        {
            ParameterD = parameterD;
            FieldOrder = fieldOrder;

            NeitralPoint = new AffineEdwardsCurvePoint(0, 1, this);
        }

        public BigInteger ParameterD { get; }

        public BigInteger FieldOrder { get; }

        public override string ToString()
        {
            return $"Edwards curve parameters: d = {ParameterD}, fieldOrder = {FieldOrder}";
        }
    }
}