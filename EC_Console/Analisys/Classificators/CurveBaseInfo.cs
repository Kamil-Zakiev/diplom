namespace Analisys.Classificators
{
    public class CurveBaseInfo
    {
        /// <summary> Порядок конечного поля </summary>
        public int FieldOrder { get; protected set; }

        /// <summary> Количество точек эллиптической кривой </summary>
        public int PointsCount { get; protected set;}

        /// <summary> Граница B </summary>
        public int EdgeB { get; protected set;}
    }
}