namespace AmeisenBotX.WowWotlk.Constants.Raids
{
    /// <summary>
    /// Eye of Eternity raid constants (WotLK 3.3.5a)
    /// </summary>
    public static class EyeOfEternity
    {
        // Boss
        public const string Malygos = "Malygos";

        // Phase 1 - Ground phase
        public const string ArcaneBreath = "Arcane Breath";
        public const string VortexDebuff = "Vortex";
        public const string PowerSpark = "Power Spark"; // Stack on Malygos

        // Phase 2 - Flying phase
        public const string SurgeOfPower = "Surge of Power";
        public const string DeepBreath = "Deep Breath";
        public const string Nexus = "Nexus Lord";
        public const string Scion = "Scion of Eternity";

        // Phase 3 - Drake phase
        public const string Wyrmrest = "Wyrmrest Skytalon";

        // Drake abilities (vehicle)
        public const string FlameSpike = "Flame Spike";      // DPS
        public const string Engulf = "Engulf in Flames";     // DoT
        public const string Revivify = "Revivify";           // Heal
        public const string LifeBurst = "Life Burst";        // AoE Heal
        public const string BlazingSpeed = "Blazing Speed";  // Movement

        // Phase 3 mechanics
        public const string StaticFieldDebuff = "Static Field";
        public const int StaticFieldSpellId = 57428;
    }
}
