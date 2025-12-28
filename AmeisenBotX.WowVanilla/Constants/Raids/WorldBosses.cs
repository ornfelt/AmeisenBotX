namespace AmeisenBotX.WowVanilla.Constants.Raids
{
    /// <summary>
    /// World bosses constants (Vanilla 1.12)
    /// </summary>
    public static class WorldBosses
    {
        // Azuregos (Azshara)
        public const string Azuregos = "Azuregos";
        public const string MarkOfFrost = "Mark of Frost"; // Can't zone back
        public const string ManaStorm = "Mana Storm";
        public const string ColdDispel = "Chill";
        public const string Cleave = "Cleave";
        public const string ReflectMagic = "Reflect";

        // Kazzak (Blasted Lands)
        public const string LordKazzak = "Lord Kazzak";
        public const string MarkOfKazzak = "Mark of Kazzak"; // Drains mana, heals boss
        public const string ShadowBoltVolley = "Shadow Bolt Volley";
        public const string CapturedSoul = "Capture Soul"; // Heals boss on player death
        public const string VoidBolt = "Void Bolt";
        public const string SupremeModeKazzak = "Supreme Mode"; // Enrage 3min

        // Dragons of Nightmare (Ashenvale, Duskwood, Feralas, Hinterlands)
        public const string Emeriss = "Emeriss";
        public const string Lethon = "Lethon";
        public const string Taerar = "Taerar";
        public const string Ysondre = "Ysondre";

        // Shared Dragon mechanics
        public const string NoxiousBreath = "Noxious Breath";
        public const string TailSweep = "Tail Sweep";
        public const string MarkOfNature = "Mark of Nature";
        public const string AuraOfNature = "Aura of Nature"; // 2 min sleep on death
        public const string SleepingFog = "Seeping Fog";

        // Emeriss mechanics
        public const string CorruptionOfEarth = "Corruption of the Earth";
        public const string VolatileInfection = "Volatile Infection"; // Spreads
        public const string PutridMushroom = "Putrid Mushroom";

        // Lethon mechanics
        public const string DrawSpirit = "Draw Spirit"; // Spawns shades
        public const string ShadowBoltWhirl = "Shadow Bolt Whirl";
        public const string SpiritShade = "Spirit Shade"; // Kill fast

        // Taerar mechanics
        public const string Bellowing = "Bellowing Roar";
        public const string ArcaneBlast = "Arcane Blast";
        public const string ShadeOfTaerar = "Shade of Taerar"; // 3 shades at 75/50/25%

        // Ysondre mechanics
        public const string LightningWave = "Lightning Wave";
        public const string SummonDemented = "Summon Demented Druid Spirit";
        public const string DementedDruidSpirit = "Demented Druid Spirit";
    }
}
