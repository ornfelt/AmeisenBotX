/// <summary>
/// This namespace contains classes and methods related to Lua scripting within the AmeisenBotX WoW bot.
/// </summary>
namespace AmeisenBotX.Wow.Shared.Lua
{
    /// <summary>
    /// Utility class for navigating and performing login actions in the game client.
    /// </summary>
    public static class LuaLogin
    {
        /// <summary>
        /// This method is used to navigate through the different screens and perform login actions in the game client.
        /// It checks if the Account Login UI is visible and shows it if it's not.
        /// If the Server Alert Frame is shown, it hides it.
        /// If the Connection Help Frame is shown, it hides it and shows the Account Login UI.
        /// If the Cinematic Frame is shown, it stops the cinematic.
        /// If the Terms of Service Frame is shown, it enables and accepts it.
        /// If the Script Errors Frame is shown, it hides it.
        /// If the Glue Dialog Frame is shown, it checks if the dialog type is "OKAY" and clicks the button if it is.
        /// If the Character Create Randomize Button is visible, it goes back to the previous screen.
        /// If the Realm List Frame is visible, it searches for the specified realm and changes to it if found.
        /// If the Character Select UI is visible, it checks if the server name includes the specified realm and selects the character slot if it does.
        /// It then clicks the CharSelectEnterWorldButton to enter the game.
        /// If the Realm List Frame is not visible, it clicks the CharSelectChangeRealmButton.
        /// If the Account Login UI is visible, it performs the login action using the provided username and password.
        /// </summary>
        public static string Get(string user, string pass, string realm, int characterslot)
        {
            // CharacterSelect_EnterWorld() got replaced by CharSelectEnterWorldButton:Click() for
            // whetever reason, the mop client freezes if we call this directly
            return @$"
                if AccountLoginUI then
                    AccountLoginUI:Show()
                end
                if ServerAlertFrame and ServerAlertFrame:IsShown() then
                    ServerAlertFrame:Hide()
                elseif ConnectionHelpFrame and ConnectionHelpFrame:IsShown() then
                    ConnectionHelpFrame:Hide()
                    AccountLoginUI:Show()
                elseif CinematicFrame and CinematicFrame:IsShown() then
                    StopCinematic()
                elseif TOSFrame and TOSFrame:IsShown() then
                    TOSAccept:Enable()
                    TOSAccept:Click()
                elseif ScriptErrors and ScriptErrors:IsShown() then
                    ScriptErrors:Hide()
                elseif GlueDialog and GlueDialog:IsShown() then
                    if GlueDialog.which == ""OKAY"" then
                        GlueDialogButton1:Click()
                    end
                elseif CharCreateRandomizeButton and CharCreateRandomizeButton:IsVisible() then
                    CharacterCreate_Back()
                elseif RealmList and RealmList:IsVisible() then
                    for a = 1, #GetRealmCategories() do
                        local found = false
                        for b = 1, GetNumRealms() do
                            if string.lower(GetRealmInfo(a, b)) == string.lower(""{realm}"") then
                                ChangeRealm(a, b)
                                RealmList: Hide()
                                found = true
                                break
                            end
                        end
                        if found then
                            break
                        end
                    end
                elseif CharacterSelectUI and CharacterSelectUI:IsVisible() then
                    if string.find(string.lower(GetServerName()), string.lower(""{realm}"")) then
                        CharacterSelect_SelectCharacter({characterslot + 1})
                        CharSelectEnterWorldButton:Click()
                    elseif RealmList and not RealmList:IsVisible() then
                         CharSelectChangeRealmButton:Click()
                    end
                elseif AccountLoginUI and AccountLoginUI:IsVisible() then
                    DefaultServerLogin(""{user}"", ""{pass}"")
                end
            ";
        }
    }
}