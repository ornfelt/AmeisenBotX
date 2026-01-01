using AmeisenBotX.Core;
using AmeisenBotX.Core.Engines.Combat.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;



namespace AmeisenBotX.ViewModels.Config
{
    public class CreateBotViewModel : INotifyPropertyChanged
    {
        private string _botName;
        private string _realm = "AmeisenRealm";
        private string _realmlist = "127.0.0.1";
        private string _username = "";
        private string _password = "";
        private string _wowPath;
        private int _characterSlot = 0;
        private bool _autoPositionWow = true;
        private bool _autoLogin = true;
        private bool _autoStartWow = true;
        private bool _autoChangeRealmlist = true;
        private bool _autoRepair = true;
        private bool _autoSell = true;
        private bool _useMounts = true;
        private CombatClassDisplayWrapper _selectedCombatClass;
        private readonly IEnumerable<CombatClassDescriptor> _availableCombatClasses;

        public CreateBotViewModel(IEnumerable<CombatClassDescriptor> availableCombatClasses, string initialWowPath = "", AmeisenBotConfig templateConfig = null)
        {
            _availableCombatClasses = availableCombatClasses;
            _wowPath = initialWowPath;

            // Apply template if provided
            if (templateConfig != null)
            {
                if (!string.IsNullOrEmpty(templateConfig.Realm))
                {
                    _realm = templateConfig.Realm;
                }

                if (!string.IsNullOrEmpty(templateConfig.Realmlist))
                {
                    _realmlist = templateConfig.Realmlist;
                }

                if (!string.IsNullOrEmpty(templateConfig.PathToWowExe))
                {
                    _wowPath = templateConfig.PathToWowExe;
                }

                _autoPositionWow = templateConfig.AutoPositionWow;
                _autoLogin = templateConfig.AutoLogin;
                _autoStartWow = templateConfig.AutostartWow;
                _autoChangeRealmlist = templateConfig.AutoChangeRealmlist;
                _autoRepair = templateConfig.AutoRepair;
                _autoSell = templateConfig.AutoSell;
                _useMounts = templateConfig.UseMounts;
            }

            // Initialize Combat Classes (Descriptors)
            List<CombatClassDisplayWrapper> list =
            [
                new CombatClassDisplayWrapper(null) // "None" option
            ];

            if (_availableCombatClasses != null)
            {
                foreach (CombatClassDescriptor cc in _availableCombatClasses)
                {
                    list.Add(new CombatClassDisplayWrapper(cc));
                }
            }
            // Keep None at top, sort others by DisplayName
            CombatClasses = list.OrderBy(x => x.Descriptor == null ? 0 : 1).ThenBy(x => x.DisplayName).ToList();
            SelectedCombatClass = CombatClasses.FirstOrDefault();
        }

        public string BotName
        {
            get => _botName;
            set { _botName = value; OnPropertyChanged(nameof(BotName)); OnPropertyChanged(nameof(CanCreate)); OnPropertyChanged(nameof(ValidationMessage)); }
        }

        public string Realm
        {
            get => _realm;
            set { _realm = value; OnPropertyChanged(nameof(Realm)); OnPropertyChanged(nameof(CanCreate)); OnPropertyChanged(nameof(ValidationMessage)); }
        }

        public string WowPath
        {
            get => _wowPath;
            set
            {
                _wowPath = value;
                OnPropertyChanged(nameof(WowPath));
                OnPropertyChanged(nameof(CanCreate));
                OnPropertyChanged(nameof(ValidationMessage));

                // Auto-populate Realmlist from realmlist.wtf if field is empty
                TryAutoPopulateRealmlist();
            }
        }

        private void TryAutoPopulateRealmlist()
        {
            // Only auto-populate if Realmlist is at default or empty
            if (!string.IsNullOrWhiteSpace(WowPath) &&
                (string.IsNullOrWhiteSpace(Realmlist) || Realmlist == "127.0.0.1"))
            {
                try
                {
                    string wowDir = Path.GetDirectoryName(WowPath);
                    string realmlistPath = FindRealmlistFile(wowDir);

                    if (realmlistPath != null && File.Exists(realmlistPath))
                    {
                        string[] lines = File.ReadAllLines(realmlistPath);
                        foreach (string line in lines)
                        {
                            // Look for "set realmlist <address>"
                            string trimmed = line.Trim();
                            if (trimmed.StartsWith("set realmlist", StringComparison.OrdinalIgnoreCase))
                            {
                                string[] parts = trimmed.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length >= 3)
                                {
                                    Realmlist = parts[2];
                                    return;
                                }
                            }
                        }
                    }
                }
                catch { /* Silently ignore parse errors */ }
            }
        }

        private static string FindRealmlistFile(string wowDir)
        {
            // Search paths in order of likelihood
            string[] searchPaths = new[]
            {
                wowDir,                                    // WoW root
                Path.Combine(wowDir, "Data"),              // Data folder
                Path.Combine(wowDir, "Data", "enUS"),      // Data/enUS
                Path.Combine(wowDir, "Data", "enGB"),      // Data/enGB
                Path.Combine(wowDir, "Data", "deDE"),      // Data/deDE
            };

            foreach (string searchPath in searchPaths)
            {
                if (!Directory.Exists(searchPath))
                {
                    continue;
                }

                // Case-insensitive search for realmlist.wtf
                try
                {
                    string[] files = Directory.GetFiles(searchPath, "realmlist.wtf", SearchOption.TopDirectoryOnly);
                    if (files.Length > 0)
                    {
                        return files[0];
                    }
                }
                catch { /* Ignore access errors */ }
            }

            return null;
        }

        public string Realmlist
        {
            get => _realmlist;
            set { _realmlist = value; OnPropertyChanged(nameof(Realmlist)); }
        }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); OnPropertyChanged(nameof(CanCreate)); OnPropertyChanged(nameof(ValidationMessage)); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); OnPropertyChanged(nameof(CanCreate)); OnPropertyChanged(nameof(ValidationMessage)); }
        }

        public int CharacterSlot
        {
            get => _characterSlot;
            set { _characterSlot = value; OnPropertyChanged(nameof(CharacterSlot)); }
        }

        public bool AutoPositionWow
        {
            get => _autoPositionWow;
            set { _autoPositionWow = value; OnPropertyChanged(nameof(AutoPositionWow)); }
        }

        public bool AutoLogin
        {
            get => _autoLogin;
            set
            {
                _autoLogin = value;
                OnPropertyChanged(nameof(AutoLogin));
                OnPropertyChanged(nameof(CanCreate));
                OnPropertyChanged(nameof(ValidationMessage));
            }
        }

        public bool AutoStartWow
        {
            get => _autoStartWow;
            set
            {
                _autoStartWow = value;
                OnPropertyChanged(nameof(AutoStartWow));
                OnPropertyChanged(nameof(CanCreate));
                OnPropertyChanged(nameof(ValidationMessage));
            }
        }

        public bool AutoChangeRealmlist
        {
            get => _autoChangeRealmlist;
            set { _autoChangeRealmlist = value; OnPropertyChanged(nameof(AutoChangeRealmlist)); }
        }

        public bool AutoRepair
        {
            get => _autoRepair;
            set { _autoRepair = value; OnPropertyChanged(nameof(AutoRepair)); }
        }

        public bool AutoSell
        {
            get => _autoSell;
            set { _autoSell = value; OnPropertyChanged(nameof(AutoSell)); }
        }

        public bool UseMounts
        {
            get => _useMounts;
            set { _useMounts = value; OnPropertyChanged(nameof(UseMounts)); }
        }

        public List<CombatClassDisplayWrapper> CombatClasses { get; }

        public CombatClassDisplayWrapper SelectedCombatClass
        {
            get => _selectedCombatClass;
            set { _selectedCombatClass = value; OnPropertyChanged(nameof(SelectedCombatClass)); }
        }

        public bool CanCreate
        {
            get
            {
                // Bot name is always required
                if (string.IsNullOrWhiteSpace(BotName))
                {
                    return false;
                }

                // If auto-start WoW, need valid WoW path
                if (AutoStartWow)
                {
                    if (string.IsNullOrWhiteSpace(WowPath) || !File.Exists(WowPath))
                    {
                        return false;
                    }
                }

                // If auto-login, need credentials
                if (AutoLogin)
                {
                    if (string.IsNullOrWhiteSpace(Username) ||
                        string.IsNullOrWhiteSpace(Password) ||
                        string.IsNullOrWhiteSpace(Realm))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public string ValidationMessage
        {
            get
            {
                if (string.IsNullOrWhiteSpace(BotName))
                {
                    return "⚠️ Enter a bot name";
                }

                if (AutoStartWow && string.IsNullOrWhiteSpace(WowPath))
                {
                    return "⚠️ Auto-start requires WoW path";
                }

                if (AutoStartWow && !string.IsNullOrWhiteSpace(WowPath) && !File.Exists(WowPath))
                {
                    return "⚠️ WoW executable not found";
                }

                if (AutoLogin)
                {
                    if (string.IsNullOrWhiteSpace(Username))
                    {
                        return "⚠️ Auto-login requires username";
                    }

                    if (string.IsNullOrWhiteSpace(Password))
                    {
                        return "⚠️ Auto-login requires password";
                    }

                    if (string.IsNullOrWhiteSpace(Realm))
                    {
                        return "⚠️ Auto-login requires realm";
                    }
                }

                return "✅ Ready to create";
            }
        }

        public AmeisenBotConfig CreateConfig()
        {
            return new AmeisenBotConfig
            {
                Realm = this.Realm,
                Realmlist = this.Realmlist,
                PathToWowExe = this.WowPath,
                BuiltInCombatClassName = this.SelectedCombatClass?.ConfigValue ?? string.Empty,
                AutoPositionWow = this.AutoPositionWow,
                Username = this.Username,
                Password = this.Password,
                CharacterSlot = this.CharacterSlot,
                AutoLogin = this.AutoLogin,
                AutostartWow = this.AutoStartWow,
                AutoChangeRealmlist = this.AutoChangeRealmlist,
                AutoRepair = this.AutoRepair,
                AutoSell = this.AutoSell,
                UseMounts = this.UseMounts
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public class CombatClassDisplayWrapper
        {
            public CombatClassDescriptor Descriptor { get; }

            public CombatClassDisplayWrapper(CombatClassDescriptor descriptor)
            {
                Descriptor = descriptor;
            }

            public string DisplayName => Descriptor == null ? "None" : $"{Descriptor.DisplayName} ({Descriptor.Author})";
            public string ConfigValue => Descriptor?.TypeName ?? string.Empty;

            public override string ToString()
            {
                return DisplayName;
            }
        }
    }
}
