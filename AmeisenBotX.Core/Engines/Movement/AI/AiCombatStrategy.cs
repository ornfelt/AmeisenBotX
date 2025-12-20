namespace AmeisenBotX.Core.Engines.Movement.AI
{
    public enum AiCombatStrategy
    {
        Unknown = 0,

        /// <summary>
        /// Fight is trivial. Save Cooldowns. Efficient rotation.
        /// </summary>
        Farm = 1,

        /// <summary>
        /// Standard combat. Use normal rotation.
        /// </summary>
        Standard = 2,

        /// <summary>
        /// Difficult fight. Use Offensive Cooldowns (Bloodlust, etc).
        /// </summary>
        Burst = 3,

        /// <summary>
        /// Health is critical. Focus on Defense/Healing.
        /// </summary>
        Survival = 4,

        /// <summary>
        /// Win unlikely. Escape immediately.
        /// </summary>
        Flee = 5,

        /// <summary>
        /// Target is casting. Interrupt immediately!
        /// </summary>
        Interrupt = 6
    }
}
