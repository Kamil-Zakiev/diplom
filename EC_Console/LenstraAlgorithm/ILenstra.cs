namespace LenstraAlgorithm
{
    using System;
    using System.Numerics;
    using System.Threading;
    using Dto;

    /// <summary> Сервис, предоставляющий методы для запуска алгоритма Ленстры </summary>
    public interface ILenstra
    {
        /// <summary> Факторизовать число n </summary>
        /// <param name="n">Факторизуемое число</param>
        /// <param name="random">Компонент случайности</param>
        /// <returns>Результатат факторизации</returns>
        LenstraFactorizationResult GetDivider(BigInteger n, Random random);

        /// <summary> Факторизовать число n с поддержкой отмены операции</summary>
        /// <param name="n">Факторизуемое число</param>
        /// <param name="random">Компонент случайности</param>
        /// <param name="token">Токен отмены</param>
        /// <returns>Результатат факторизации</returns>
        LenstraFactorizationResult GetDividerWithCancel(BigInteger n, Random random, CancellationToken token);
    }
}