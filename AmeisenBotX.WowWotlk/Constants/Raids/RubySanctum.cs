namespace AmeisenBotX.WowWotlk.Constants.Raids
{
    /// <summary>
    /// Ruby Sanctum raid constants (WotLK 3.3.5a)
    /// </summary>
    public static class RubySanctum
    {
        // Mini-bosses
        public const string Baltharus = "Baltharus the Warborn";
        public const string Saviana = "Saviana Ragefire";
        public const string Zarithrian = "General Zarithrian";

        // Main boss
        public const string Halion = "Halion";

        // Baltharus mechanics
        public const string Enervating = "Enervating Brand";
        public const string Repelling = "Repelling Wave";
        public const string BaltharusClone = "Baltharus the Warborn"; // Splits

        // Saviana mechanics
        public const string ConflagrationDebuff = "Conflagration";
        public const string FlameSavage = "Flame Breath";

        // Zarithrian mechanics
        public const string CleaveZarith = "Cleave";
        public const string OnyxFlameCaller = "Onyx Flamecaller"; // Adds

        // Halion mechanics - Twilight realm
        public const string FieryDebuff = "Fiery Combustion";     // Physical realm
        public const string SoulDebuff = "Soul Consumption";       // Twilight realm
        public const string TwilightCutter = "Twilight Cutter";    // Laser beam
        public const string Corporeality = "Corporeality";         // Balance mechanic

        public const int FieryCombustionSpellId = 74562;
        public const int SoulConsumptionSpellId = 74792;
        public const int TwilightCutterSpellId = 74769;

        // Realm portals
        public const string TwilightPortal = "Twilight Portal";
    }
}
