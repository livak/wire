using System.Linq;
using System.Windows;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int brojZavoja;
            int maxOdstupanje;
            int maxBrojZica;
            double povrsinaUtora;
            double presjek;

            int slojnost = radio_Slojnost1.IsChecked.Value ? 1 : 2;

            int from = drop_From.SelectedIndex;
            int to = drop_To.SelectedIndex;

            bool sveKombinacije = cbx_PrikaziSve.IsChecked ?? false;
            var maxRazmak = sveKombinacije ? 3 : 1;

            var currentSeparator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            int.TryParse(this.tbx_BrojZavoja.Text, out brojZavoja);
            int.TryParse(this.tbx_MaxOdstupanje.Text, out maxOdstupanje);
            int.TryParse(this.tbx_MaxBrojZica.Text, out maxBrojZica);
            double.TryParse(this.tbx_PovrsinaUtora.Text.Replace(".", currentSeparator).Replace(",", currentSeparator), out povrsinaUtora);
            double.TryParse(this.tbx_Presjek.Text.Replace(".", currentSeparator).Replace(",", currentSeparator), out presjek);

            var inputParams = new InputParams
            {
                BrojZavoja = brojZavoja,
                MaxBrojZica = maxBrojZica,
                MaxOdstupanje = maxOdstupanje,
                MaxRazmak = maxRazmak,
                PovrsinaUtora = povrsinaUtora,
                Presjek = presjek,
                Slojnost = slojnost
            };

            var calculator = new WireCalculator(promjeriZica: Configuration.Zice.Get(from, to));

            this.grid_Rezultat.ItemsSource = calculator
                .GetResults(inputParams)
                .OrderBy(x => x.Razmak)
                .ThenBy(x => x.Odstupanje);
        }
    }
}
