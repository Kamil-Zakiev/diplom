using System.Numerics;
using LenstraAlgorithm;

namespace EdwardsCurves
{
    public class EdwardsCurve : IEdwardsCurve
    {
        public EdwardsCurve(BigInteger parameterD, BigInteger fieldOrder)
        {
            ParameterD = parameterD;
            FieldOrder = fieldOrder;

            NeitralPoint = new EdwardsCurvePoint(0, 1, this);
        }

        public BigInteger ParameterD { get; }
        
        public BigInteger FieldOrder { get; }
        
        public readonly EdwardsCurvePoint NeitralPoint;

        public override string ToString()
        {
            return $"Edwards curve parameters: d = {ParameterD}, fieldOrder = {FieldOrder}";
        }
    }
}