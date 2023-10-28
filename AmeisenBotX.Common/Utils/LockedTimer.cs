using System;
using System.Threading;

/// <summary>
/// Contains utility classes and methods used by the AmeisenBotX application.
/// </summary>
namespace AmeisenBotX.Common.Utils
{
    /// <summary>
    /// Provides a timer mechanism that ensures only one tick operation is active at a time.
    /// </summary>
    public class LockedTimer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LockedTimer"/> class with the specified tick interval and callback actions.
        /// </summary>
        /// <param name="tickMs">The time interval, in milliseconds, between tick events.</param>
        /// <param name="actions">A list of callback methods to be executed on each tick.</param>
        private int timerBusy;

        /// <summary>
        /// Utility class to create a ticker which can only run one tick at a time.
        /// </summary>
        /// <param name="tickMs">Milliseconds interval</param>
        /// <param name="actions">Callbacks</param>
        public LockedTimer(int tickMs, params Action[] actions)
        {
            Timer = new(Tick, null, 0, tickMs);

            foreach (Action a in actions)
            {
                OnTick += a;
            }
        }

        /// <summary>
        /// Finalizes the <see cref="LockedTimer"/> instance, ensuring the underlying timer resources are released.
        /// </summary>
        ~LockedTimer()
        {
            Timer.Dispose();
        }

        /// <summary>
        /// Occurs when the timer ticks.
        /// </summary>
        public event Action OnTick;

        /// <summary>
        /// Gets the underlying timer instance.
        /// </summary>
        private Timer Timer { get; }

        /// <summary>
        /// Sets the interval at which the timer ticks.
        /// </summary>
        /// <param name="tickMs">The time interval, in milliseconds, between tick events.</param>
        public void SetInterval(int tickMs)
        {
            Timer.Change(0, tickMs);
        }

        /// <summary>
        /// Handles the tick event of the timer, ensuring that the tick operation is locked to a single instance.
        /// </summary>
        /// <param name="o">State object. Not used in this method.</param>
        private void Tick(object o)
        {
            if (Interlocked.CompareExchange(ref timerBusy, 1, 0) == 0)
            {
                OnTick?.Invoke();
                timerBusy = 0;
            }
        }
    }
}