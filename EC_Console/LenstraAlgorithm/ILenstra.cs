using System;
using System.Numerics;
using System.Threading;

namespace LenstraAlgorithm
{
    public interface ILenstra
    {
        LenstraResultOfEllepticCurve GetDivider(BigInteger n, Random random);

        LenstraResultOfEllepticCurve GetDividerWithCancel(BigInteger n, Random random, CancellationToken token);
    }
}