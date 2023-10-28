/// <summary>
/// This namespace contains enums related to battlegrounds in the AmeisenBotX Core Engine.
/// </summary>
namespace AmeisenBotX.Core.Engines.Battleground.KamelBG.Enums
{
    /// <summary>
    /// Enumeration of flag IDs used in various battlegrounds.
    /// </summary>
    public enum Flags
    {
        //ArathiBasin
        NeutralFlags = 6271,

        HordFlags = 6253,
        HordFlagsAktivate = 6254,
        AlliFlags = 6251,
        AlliFlagsAktivate = 6252,

        //Eye of the Storm
        NetherstormFlag = 7153,
    }

    /// <summary>
    /// Enumeration of flag IDs specific to the Alliance faction in battlegrounds.
    /// </summary>
    public enum FlagsAlli
    {
        NeutralFlags = 6271,
        AlliFlags = 6251,
        AlliFlagsAktivate = 6252
    }

    /// <summary>
    /// Enumeration of flag IDs specific to the Horde faction in battlegrounds.
    /// </summary>
    public enum FlagsHord
    {
        NeutralFlags = 6271,
        HordFlags = 6253,
        HordFlagsAktivate = 6254
    }
}