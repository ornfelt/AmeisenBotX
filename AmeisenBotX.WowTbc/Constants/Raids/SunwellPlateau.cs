namespace AmeisenBotX.WowTbc.Constants.Raids
{
    /// <summary>
    /// Sunwell Plateau raid constants (TBC 2.4.3)
    /// </summary>
    public static class SunwellPlateau
    {
        // Bosses
        public const string Kalecgos = "Kalecgos";
        public const string Sathrovarr = "Sathrovarr the Corruptor";
        public const string Brutallus = "Brutallus";
        public const string Felmyst = "Felmyst";
        public const string EredarTwins = "Eredar Twins";
        public const string Muru = "M'uru";
        public const string Entropius = "Entropius";
        public const string KilJaeden = "Kil'jaeden";

        // Kalecgos mechanics
        public const string ArcaneBuffet = "Arcane Buffet";
        public const string FrostBreathKal = "Frost Breath";
        public const string SpectralBlast = "Spectral Blast"; // Teleport to demon realm
        public const string Crazed = "Crazed";
        // Demon realm
        public const string CorruptingStrike = "Corrupting Strike";
        public const string Curse = "Curse of Boundless Agony";

        // Brutallus mechanics (DPS check)
        public const string MeteorSlash = "Meteor Slash"; // Stack tanks
        public const string Burn = "Burn"; // DoT stacking
        public const string Stomp = "Stomp";
        public const int BurnSpellId = 46394;

        // Felmyst mechanics
        public const string GasNova = "Gas Nova";
        public const string Encapsulate = "Encapsulate";
        public const string Corrosion = "Corrosion";
        public const string DemonicVapor = "Demonic Vapor"; // Fog
        public const string FogOfCorruption = "Fog of Corruption"; // MC
        public const string DeepBreath = "Deep Breath"; // Line attack
        public const string Unyielding = "Unyielding Dead"; // Skeletons

        // Eredar Twins mechanics
        public const string Alythess = "Lady Sacrolash";
        public const string Sacrolash = "Grand Warlock Alythess";
        // Sacrolash abilities
        public const string ShadowBlades = "Shadow Blades";
        public const string ShadowNova = "Shadow Nova";
        public const string ConfoundingBlow = "Confounding Blow";
        public const string DarkStrike = "Dark Strike";
        // Alythess abilities
        public const string PyrogenicsSpell = "Pyrogenics";
        public const string Conflagration = "Conflagration";
        public const string BlazeSpell = "Blaze";
        public const string FlameTouch = "Flame Touched";

        // M'uru mechanics
        public const string DarknessSpell = "Darkness";
        public const string NegativeEnergy = "Negative Energy";
        public const string VoidZone = "Void Zone";
        public const string DarkFiend = "Dark Fiend";
        public const string VoidSentinel = "Void Sentinel";
        public const string SchwingOrb = "Schwing";
        // Adds
        public const string BerserkerTBC = "Shadowsword Berserker";
        public const string FuryMage = "Shadowsword Fury Mage";

        // Kil'jaeden mechanics (5 phases)
        public const string SoulFlay = "Soul Flay";
        public const string LegionLightning = "Legion Lightning";
        public const string FireBloom = "Fire Bloom";
        public const string ShadowSpike = "Shadow Spike";
        public const string FlameDart = "Flame Dart";
        public const string Darkness = "Darkness of a Thousand Souls"; // Run to orbs!
        public const string ArmagedonSpell = "Armageddon";
        // Shield orbs
        public const string ShieldOrb = "Shield Orb";
        public const string SinisterReflection = "Sinister Reflection"; // Player clones
        // Dragon allies
        public const string Kalec = "Kalecgos"; // Dragon form, helps
        public const string Anveena = "Anveena"; // Final phase sacrifice
    }
}
