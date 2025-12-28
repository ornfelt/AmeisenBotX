namespace AmeisenBotX.WowWotlk.Constants.Raids
{
    /// <summary>
    /// Naxxramas raid constants (WotLK 3.3.5a)
    /// </summary>
    public static class Naxxramas
    {
        // =============================================
        // ARACHNID QUARTER
        // =============================================
        public const string AnubRekhan = "Anub'Rekhan";
        public const string GrandWidowFaerlina = "Grand Widow Faerlina";
        public const string Maexxna = "Maexxna";

        public static class AnubRekhanWotlk
        {
            public const int LocustSwarmSpellId = 28785; // Verify ID, using common one for 10-man
            public const int ImpaleSpellId = 28783;
        }

        // Maexxna mechanics
        public const string WebWrap = "Web Wrap";
        public const int WebWrapSpellId = 28622;

        // =============================================
        // PLAGUE QUARTER
        // =============================================
        public const string NothThePlaguebringer = "Noth the Plaguebringer";
        public const string HeiganTheUnclean = "Heigan the Unclean";
        public const string Loatheb = "Loatheb";

        // Heigan mechanics - dance floor
        public const string Eruption = "Eruption";

        // =============================================
        // MILITARY QUARTER
        // =============================================
        public const string InstructorRazuvious = "Instructor Razuvious";
        public const string GothikTheHarvester = "Gothik the Harvester";
        public const string TheFourHorsemen = "The Four Horsemen";
        public const string BaronRivendare = "Baron Rivendare";
        public const string ThaneLordKorthazz = "Thane Korth'azz";
        public const string LadyBlaumeux = "Lady Blaumeux";
        public const string SirZeliek = "Sir Zeliek";

        // Four Horsemen marks
        public const string MarkOfRivendare = "Mark of Rivendare";
        public const string MarkOfKorthazz = "Mark of Korth'azz";
        public const string MarkOfBlaumeux = "Mark of Blaumeux";
        public const string MarkOfZeliek = "Mark of Zeliek";

        // =============================================
        // CONSTRUCT QUARTER
        // =============================================
        public const string Patchwerk = "Patchwerk";
        public const string Grobbulus = "Grobbulus";
        public const string Gluth = "Gluth";
        public const string Thaddius = "Thaddius";
        public const string Feugen = "Feugen";
        public const string Stalagg = "Stalagg";

        // Thaddius polarity mechanics
        public const string PositiveCharge = "Positive Charge";
        public const string NegativeCharge = "Negative Charge";
        public const int PositiveChargeSpellId = 28059;
        public const int NegativeChargeSpellId = 28084;

        // Grobbulus mechanics
        public const string MutatingInjection = "Mutating Injection";

        // =============================================
        // FROSTWYRM LAIR
        // =============================================
        public const string Sapphiron = "Sapphiron";
        public const string KelThuzad = "Kel'Thuzad";

        // Sapphiron mechanics
        public const string FrostBreath = "Frost Breath";
        public const string IceBolt = "Ice Bolt";
        public const string FrostAura = "Frost Aura";

        // Kel'Thuzad mechanics
        public const string FrostBlast = "Frost Blast";
        public const string ShadowFissure = "Shadow Fissure";
    }
}
