namespace LenstraAlgorithm
{
    using System.Numerics;

    /// <summary> Эллиптическая кривая </summary>
    public interface IEllepticCurve
    {
        /// <summary> Размерность поля, в котором находятся координаты точек эллиптической кривой </summary>
        BigInteger FieldOrder { get; }
    }
}