namespace Analisys
{
    /// <summary> Класс эллиптической кривой </summary>
    public class CurveClass
    {
        /// <summary> Номер класса </summary>
        public int ClassNumber { get; set; }

        /// <summary> Имя класса </summary>
        public string ClassName => $"Class #{ClassNumber}";
    }
}