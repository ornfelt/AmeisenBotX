using AmeisenBotX.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Input;

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

        protected virtual void BrowseFile(object obj)
        {
            OpenFileDialog openFileDialog = new();
            ConfigureFileDialog(openFileDialog);

            string current = Value as string;
            if (!string.IsNullOrWhiteSpace(current))
            {
                try
                {
                    string dir = System.IO.Path.GetDirectoryName(current);
                    if (System.IO.Directory.Exists(dir))
                    {
                        openFileDialog.InitialDirectory = dir;
                    }
                }
                catch { }
            }

            if (openFileDialog.ShowDialog() == true)
            {
                Value = openFileDialog.FileName;
            }
        }

        protected virtual void ConfigureFileDialog(Microsoft.Win32.OpenFileDialog dialog)
        {
            // Default behavior: all files? or inherited specific?
        }
    }

    public class ImageConfigProperty : FileConfigProperty
    {
        private const int PortraitSize = 384;

        public ICommand ClearCommand { get; }

        public ImageConfigProperty(AmeisenBotConfig config, PropertyInfo property) : base(config, property)
        {
            ClearCommand = new RelayCommand(ClearValue);
        }

        protected override void ConfigureFileDialog(Microsoft.Win32.OpenFileDialog dialog)
        {
            dialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All Files|*.*";
        }

        protected override void BrowseFile(object obj)
        {
            OpenFileDialog dialog = new();
            ConfigureFileDialog(dialog);

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            // Process and save directly as portrait.png
            string portraitPath = GetPortraitPath();
            if (portraitPath != null && ProcessAndSaveImage(dialog.FileName, portraitPath))
            {
                Value = portraitPath;
            }
        }

        private string GetPortraitPath()
        {
            try
            {
                string configDir = System.IO.Path.GetDirectoryName(Config.Path);
                return System.IO.Path.Combine(configDir, "portrait.png");
            }
            catch { return null; }
        }

        private bool ProcessAndSaveImage(string sourcePath, string destPath)
        {
            try
            {
                using Image original = System.Drawing.Image.FromFile(sourcePath);

                // Center-crop to square
                int size = Math.Min(original.Width, original.Height);
                int x = (original.Width - size) / 2;
                int y = (original.Height - size) / 2;

                using Bitmap result = new(PortraitSize, PortraitSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = System.Drawing.Graphics.FromImage(result))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.DrawImage(original,
                        new System.Drawing.Rectangle(0, 0, PortraitSize, PortraitSize),
                        new System.Drawing.Rectangle(x, y, size, size),
                        System.Drawing.GraphicsUnit.Pixel);
                }

                result.Save(destPath, System.Drawing.Imaging.ImageFormat.Png);
                return true;
            }
            catch { return false; }
        }

        private void ClearValue(object obj)
        {
            // Delete portrait.png to trigger auto-regeneration on next launch
            string portraitPath = GetPortraitPath();
            if (portraitPath != null)
            {
                try { System.IO.File.Delete(portraitPath); } catch { }
            }

            // Setting Value to empty will make UseCustomPortrait return false (it's computed)
            Value = string.Empty;
            OnPropertyChanged(nameof(HasValue));
        }

        public bool HasValue => !string.IsNullOrWhiteSpace(Value as string);
    }

    public class SelectionConfigProperty : StringConfigProperty
    {
        public IEnumerable<string> Options { get; }

        public SelectionConfigProperty(AmeisenBotConfig config, PropertyInfo property, IEnumerable<string> options) : base(config, property)
        {
            Options = options;
        }
    }

    public interface IConfigItem
    {
        string DisplayName { get; }
        string ConfigValue { get; }
    }

    public class ObjectSelectionConfigProperty : ConfigPropertyViewModel
    {
        public IEnumerable<IConfigItem> Options { get; }

        public ObjectSelectionConfigProperty(AmeisenBotConfig config, PropertyInfo property, IEnumerable<IConfigItem> options) : base(config, property)
        {
            Options = options;
        }

        public override object Value
        {
            get
            {
                string currentConfigValue = (string)Property.GetValue(Config);
                // Return the matching IConfigItem, or null/first if not found
                foreach (IConfigItem item in Options)
                {
                    if (item.ConfigValue == currentConfigValue)
                    {
                        return item;
                    }
                }

                // Fallback for empty/mismatch: preserve null or try to valid default?
                // Returning null might show empty in ComboBox, which is fine.
                return null;
            }
            set
            {
                if (value is IConfigItem item)
                {
                    Property.SetValue(Config, item.ConfigValue);
                }
                else if (value == null)
                {
                    Property.SetValue(Config, string.Empty);
                }

                OnPropertyChanged(nameof(Value));
            }
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
    public class PasswordConfigProperty : StringConfigProperty
    {
        public PasswordConfigProperty(AmeisenBotConfig config, PropertyInfo property) : base(config, property) { }
    }
}
