namespace EdwardsCurves
{
    using System.Numerics;
    using LenstraAlgorithm;

    /// <summary> Кривая Эдвардса </summary>
    public interface IEdwardsCurve : IEllepticCurve
    {
        /// <summary> Параметр d кривой Эдвардса </summary>
        BigInteger ParameterD { get; }
    }
}