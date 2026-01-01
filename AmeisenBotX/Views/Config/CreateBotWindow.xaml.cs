using AmeisenBotX.Core;
using AmeisenBotX.Core.Engines.Combat.Classes;
using AmeisenBotX.ViewModels.Config;
using Microsoft.Win32;
using System.Windows;

namespace AmeisenBotX.Views.Config
{
    public partial class CreateBotWindow : Window
    {
        private readonly CreateBotViewModel _viewModel;

        public CreateBotWindow(System.Collections.Generic.IEnumerable<CombatClassDescriptor> combatClasses, string initialWowPath = "", AmeisenBotConfig templateConfig = null)
        {
            InitializeComponent();
            _viewModel = new CreateBotViewModel(combatClasses, initialWowPath, templateConfig);
            DataContext = _viewModel;

            // Pre-fill PasswordBox if template provided
            if (templateConfig != null && !string.IsNullOrEmpty(templateConfig.Password))
            {
                txtPassword.Password = templateConfig.Password;
            }
        }

        public AmeisenBotConfig CreatedConfig => _viewModel.CreateConfig();
        public string BotName => _viewModel.BotName;

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CanCreate)
            {
                // Password is already in ViewModel via binding/events
                DialogResult = true;
                Close();
            }
        }

        private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "World of Warcraft (Wow.exe)|Wow.exe|All Files (*.*)|*.*",
                Title = "Select World of Warcraft Executable"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _viewModel.WowPath = openFileDialog.FileName;
            }
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Visibility == Visibility.Visible)
            {
                _viewModel.Password = txtPassword.Password;
            }
        }

        private void BtnShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (btnShowPassword.IsChecked == true)
            {
                // Switch to Visible (TextBox)
                txtVisiblePassword.Visibility = Visibility.Visible;
                txtPassword.Visibility = Visibility.Collapsed;
                // TextBox is bound to ViewModel, and PasswordBox updated ViewModel, so TextBox should have correct value via binding.
                // Just in case, we can force a sync if binding hasn't fired yet? 
                // Binding usually works, but safe to do nothing as VM is source of truth.
            }
            else
            {
                // Switch to Hidden (PasswordBox)
                txtVisiblePassword.Visibility = Visibility.Collapsed;
                txtPassword.Visibility = Visibility.Visible;
                // Sync PasswordBox from ViewModel (which was updated by TextBox)
                txtPassword.Password = _viewModel.Password;
            }
        }


    }
}
