using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Wire
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainWindowViewModel();
            main.DataContext = viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.DoCalculation();
        }

        private void ent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                viewModel.DoCalculation();
            }
        }

        private void removeZica_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ButtonBase;
            if (button == null || button.CommandParameter == null) return;

            viewModel.Zice.Remove((double)removeZica.CommandParameter);
            Configuration.SaveZice(viewModel.Zice);
        }

        private void addZica_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ButtonBase;
            if (button == null) return;
            var value = button.CommandParameter as string;
            if (value == null) return;

            var zica = MainWindowViewModel.ParseDouble(value);
            if (viewModel.Zice.Contains(zica)) return;
            viewModel.Zice.AddInOrder(zica);
            Configuration.SaveZice(viewModel.Zice);
        }

        private void resetAll_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Zice = new ObservableCollection<double>(Configuration.GetDefaults());
            Configuration.SaveZice(viewModel.Zice);
        }
    }
}
