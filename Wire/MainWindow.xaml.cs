using System.Windows;
using System.Windows.Data;
using Wire.Properties;

namespace Wire
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void window_Closed(object sender, System.EventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
