using System.Numerics;
using LenstraAlgorithm;

namespace EdwardsCurves
{
    public class ProjectiveEdwardsCurve : IEllepticCurve
    {
        public ProjectiveEdwardsCurve(BigInteger parameterD, BigInteger fieldOrder)
        {
            ParameterD = parameterD;
            FieldOrder = fieldOrder;

            Infinity2 = new ProjectiveEdwardsCurvePoint(0, 1, 0, this);
            Infinity1 = new ProjectiveEdwardsCurvePoint(1, 0, 0, this);
            NeitralPoint = new ProjectiveEdwardsCurvePoint(0, 1, 1, this);
        }

        public BigInteger ParameterD { get; }
        
        public BigInteger FieldOrder { get; }
        
        public readonly ProjectiveEdwardsCurvePoint Infinity1;

        public readonly ProjectiveEdwardsCurvePoint Infinity2;
        
        public readonly ProjectiveEdwardsCurvePoint NeitralPoint;
        
        

    }
}