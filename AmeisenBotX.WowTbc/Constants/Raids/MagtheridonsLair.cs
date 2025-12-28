namespace AmeisenBotX.WowTbc.Constants.Raids
{
    /// <summary>
    /// Magtheridon's Lair raid constants (TBC 2.4.3)
    /// </summary>
    public static class MagtheridonsLair
    {
        // Boss
        public const string Magtheridon = "Magtheridon";
        public const string Channeler = "Hellfire Channeler";

        // Phase 1 - Channelers
        public const string SoulTransfer = "Soul Transfer";
        public const string DarkMending = "Dark Mending";
        public const string ShadowBoltVolley = "Shadow Bolt Volley";
        public const string BurningAbyssal = "Burning Abyssal";

        // Cube mechanic
        public const string ManticorenCube = "Manticron Cube";
        public const string Banish = "Banishment"; // Click cubes!

        // Magtheridon mechanics
        public const string Cleave = "Cleave";
        public const string QuakeDebuff = "Quake"; // Ceiling falls
        public const string BlastWave = "Blast Nova"; // Channel cubes!
        public const string DebrisDebuff = "Debris";
        public const int BlastNovaSpellId = 30616;
    }
}
