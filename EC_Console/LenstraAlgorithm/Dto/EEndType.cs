namespace LenstraAlgorithm.Dto
{
    /// <summary> Контекст остановки факторизации </summary>
    public enum EEndType
    {
        /// <summary> Алгоритм отработал доконца </summary>
        RunToComplete = 10,
        
        /// <summary> Алгоритм был отменен </summary>
        Cancelled = 20
    }
}