using System.Numerics;

namespace LenstraAlgorithm
{
    public interface IEllepticCurve
    {
        BigInteger FieldOrder { get; }
    }
}