using AmeisenBotX.Memory;
using AmeisenBotX.Wow.Offsets;

/// <summary>
/// Represents a namespace for the AmeisenBotX.Wow namespace that includes a WowMemoryApi class that extends XMemory and initializes a new instance with the specified offset list.
/// </summary>
namespace AmeisenBotX.Wow
{
    /// <summary>
    /// Represents a WowMemoryApi class that extends XMemory and initializes a new instance with the specified offset list.
    /// </summary>
    public class WowMemoryApi : XMemory
    {
        /// <summary>
        /// Initializes a new instance of the WowMemoryApi class with the specified offset list.
        /// </summary>
        public WowMemoryApi(IOffsetList offsets)
                    : base()
        {
            Offsets = offsets;
        }

        /// <summary>
        /// Gets the list of offsets.
        /// </summary>
        public IOffsetList Offsets { get; }
    }
}