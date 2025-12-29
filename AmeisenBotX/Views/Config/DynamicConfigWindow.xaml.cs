using System.Windows;
using System.Windows.Input;
using AmeisenBotX.Core;
using AmeisenBotX.ViewModels.Config;

namespace AmeisenBotX.Views.Config
{
    public partial class DynamicConfigWindow : Window
    {
        private readonly DynamicConfigViewModel _viewModel;

        public DynamicConfigWindow(AmeisenBotConfig config)
        {
            InitializeComponent();
            _viewModel = new DynamicConfigViewModel(config);
            DataContext = _viewModel;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
}
