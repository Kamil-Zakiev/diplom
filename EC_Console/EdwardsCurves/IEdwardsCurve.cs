using System.Numerics;
using LenstraAlgorithm;

namespace EdwardsCurves
{
    public interface IEdwardsCurve : IEllepticCurve
    {
        BigInteger ParameterD { get; }
    }
}