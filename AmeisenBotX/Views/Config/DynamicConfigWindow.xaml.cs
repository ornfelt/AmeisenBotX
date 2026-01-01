using AmeisenBotX.Core;
using AmeisenBotX.ViewModels.Config;
using System.Windows;
using System.Windows.Controls;

namespace AmeisenBotX.Views.Config
{
    public partial class DynamicConfigWindow : Window
    {
        private readonly DynamicConfigViewModel _viewModel;

        public DynamicConfigWindow(AmeisenBotConfig config, System.Collections.Generic.IEnumerable<AmeisenBotX.Core.Engines.Combat.Classes.ICombatClass> combatClasses)
        {
            InitializeComponent();
            _viewModel = new DynamicConfigViewModel(config, combatClasses);
            DataContext = _viewModel;
        }



        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            // Changes are already applied to the Config object by the ViewModel bindings.
            // Just close the window. The caller (App or MainWindow) should handle saving the Config to disk if needed.
            DialogResult = true;
            Close();
        }
    }

    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordBoxHelper),
                new PropertyMetadata(false, Attach));

        public static void SetAttach(DependencyObject dp, bool value) => dp.SetValue(AttachProperty, value);
        public static bool GetAttach(DependencyObject dp) => (bool)dp.GetValue(AttachProperty);
        public static string GetPassword(DependencyObject dp) => (string)dp.GetValue(PasswordProperty);
        public static void SetPassword(DependencyObject dp, string value) => dp.SetValue(PasswordProperty, value);

        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not System.Windows.Controls.PasswordBox passwordBox)
            {
                return;
            }

            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as System.Windows.Controls.PasswordBox;
            SetPassword(passwordBox, passwordBox.Password);
        }

        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as System.Windows.Controls.PasswordBox;
            passwordBox.PasswordChanged -= PasswordChanged;
            if (!GetUpdatingPassword(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += PasswordChanged;
        }

        private static readonly DependencyProperty UpdatingPasswordProperty =
            DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBoxHelper));

        private static bool GetUpdatingPassword(DependencyObject dp) => (bool)dp.GetValue(UpdatingPasswordProperty);
        private static void SetUpdatingPassword(DependencyObject dp, bool value) => dp.SetValue(UpdatingPasswordProperty, value);
    }
}
