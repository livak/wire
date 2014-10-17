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
            int brojZavoja       = ParseInt(tbx_BrojZavoja.Text);
            int maxOdstupanje    = ParseInt(tbx_MaxOdstupanje.Text);
            int maxBrojZica      = ParseInt(tbx_MaxBrojZica.Text);
            double povrsinaUtora = ParseDouble(tbx_PovrsinaUtora.Text);
            double presjek       = ParseDouble(tbx_Presjek.Text);

            int slojnost = radio_Slojnost1.IsChecked.Value ? 1 : 2;

            int from = drop_From.SelectedIndex;
            int to = drop_To.SelectedIndex;

            bool sveKombinacije = cbx_PrikaziSve.IsChecked ?? false;
            var maxRazmak = sveKombinacije ? 3 : 1;
            
            var nemaZica = tbx_NemaZice.Text.Split(';',' ').Select(ParseDouble).Where(f => f != default(double));

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

            var calculator = new WireCalculator(promjeriZica: Configuration.Zice.Get(from, to).Except(nemaZica));

            grid_Rezultat.ItemsSource = calculator
                .GetResults(inputParams)
                .OrderBy(x => x.Razmak)
                .ThenBy(x => x.Odstupanje);
        }

        private double ParseDouble(string s)
        {
            double r;
            double.TryParse(ToCurrentCultureSeparator(s), out r);
            return r;
        }

        private int ParseInt(string s)
        {
            int r;
            int.TryParse(s, out r);
            return r;
        }

        private static string ToCurrentCultureSeparator(string s)
        {
            var currentSeparator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            return s.Replace(".", currentSeparator).Replace(",", currentSeparator);
        }
    }
}
