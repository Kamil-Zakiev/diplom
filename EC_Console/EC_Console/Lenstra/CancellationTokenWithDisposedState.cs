using System.Threading;

namespace EC_Console
{
    /// <summary>
    /// Собственная реализация для проверки состояния удаленности 
    /// </summary>
    public class CancellationTokenWithDisposedState : CancellationTokenSource
    {
        public CancellationTokenWithDisposedState()
        {
            IsDead = false;
        }

        public bool IsDead;

        protected override void Dispose(bool disposing)
        {
            IsDead = true;
            base.Dispose(disposing);
        }
    }
}
