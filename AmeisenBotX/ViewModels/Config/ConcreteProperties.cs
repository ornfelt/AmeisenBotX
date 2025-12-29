using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Input;
using AmeisenBotX.Core;
using AmeisenBotX.Core;

namespace AmeisenBotX.ViewModels.Config
{
    public class BoolConfigProperty : ConfigPropertyViewModel
    {
        public BoolConfigProperty(AmeisenBotConfig config, PropertyInfo property) : base(config, property) { }

        public override object Value
        {
            get => (bool)Property.GetValue(Config);
            set
            {
                Property.SetValue(Config, value);
                OnPropertyChanged(nameof(Value));
            }
        }
    }

    public class StringConfigProperty : ConfigPropertyViewModel
    {
        public StringConfigProperty(AmeisenBotConfig config, PropertyInfo property) : base(config, property) { }

        public override object Value
        {
            get => (string)Property.GetValue(Config);
            set
            {
                Property.SetValue(Config, value);
                OnPropertyChanged(nameof(Value));
            }
        }
    }

    public class IntConfigProperty : ConfigPropertyViewModel
    {
        public IntConfigProperty(AmeisenBotConfig config, PropertyInfo property) : base(config, property) { }

        public override object Value
        {
            get => (int)Property.GetValue(Config);
            set
            {
                try
                {
                    int val = Convert.ToInt32(value);
                    Property.SetValue(Config, val);
                    OnPropertyChanged(nameof(Value));
                }
                catch { }
            }
        }
    }
    
    public class FloatConfigProperty : ConfigPropertyViewModel
    {
        public FloatConfigProperty(AmeisenBotConfig config, PropertyInfo property) : base(config, property) { }

        public override object Value
        {
            get => Convert.ToSingle(Property.GetValue(Config));
            set
            {
                try
                {
                    float val = Convert.ToSingle(value);
                    Property.SetValue(Config, val);
                    OnPropertyChanged(nameof(Value));
                }
                catch { }
            }
        }
    }

    public class DoubleConfigProperty : ConfigPropertyViewModel
    {
        public DoubleConfigProperty(AmeisenBotConfig config, PropertyInfo property) : base(config, property) { }

        public override object Value
        {
            get => (double)Property.GetValue(Config);
            set
            {
                try
                {
                    double val = Convert.ToDouble(value);
                    Property.SetValue(Config, val);
                    OnPropertyChanged(nameof(Value));
                }
                catch { }
            }
        }
    }

    public class EnumConfigProperty : ConfigPropertyViewModel
    {
        public EnumConfigProperty(AmeisenBotConfig config, PropertyInfo property) : base(config, property)
        {
            Options = Enum.GetValues(Property.PropertyType);
        }

        public Array Options { get; }

        public override object Value
        {
            get => Property.GetValue(Config);
            set
            {
                Property.SetValue(Config, value);
                OnPropertyChanged(nameof(Value));
            }
        }
    }
    public class FileConfigProperty : StringConfigProperty
    {
        public ICommand BrowseCommand { get; }

        public FileConfigProperty(AmeisenBotConfig config, PropertyInfo property) : base(config, property)
        {
            BrowseCommand = new RelayCommand(BrowseFile);
        }

        private void BrowseFile(object obj)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            string current = Value as string;
            if (!string.IsNullOrWhiteSpace(current))
            {
                try
                {
                    string dir = System.IO.Path.GetDirectoryName(current);
                    if (System.IO.Directory.Exists(dir))
                        openFileDialog.InitialDirectory = dir;
                }
                catch { }
            }

            if (openFileDialog.ShowDialog() == true)
            {
                Value = openFileDialog.FileName;
            }
        }
    }

    public class SelectionConfigProperty : StringConfigProperty
    {
        public IEnumerable<string> Options { get; }

        public SelectionConfigProperty(AmeisenBotConfig config, PropertyInfo property, IEnumerable<string> options) : base(config, property)
        {
            Options = options;
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
