using AmeisenBotX.Core;

namespace AmeisenBotX.Windows.StateConfig
{
    public interface IStateConfigWindow
    {
        AmeisenBotConfig Config { get; }

        bool ShouldSave { get; }
    }
}