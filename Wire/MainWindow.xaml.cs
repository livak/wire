using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private double UkupnaPovrsina(double promjer, double brojZica)
        {
            return Povrsina(promjer) * brojZica;
        }

        private double Povrsina(double promjer)
        {
            double radijus = promjer / 2;
            return Math.Pow(radijus, 2) * Math.PI;
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

            var currentSeparator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            int.TryParse(this.tbx_BrojZavoja.Text, out brojZavoja);
            int.TryParse(this.tbx_MaxOdstupanje.Text, out maxOdstupanje);
            int.TryParse(this.tbx_MaxBrojZica.Text, out maxBrojZica);
            double.TryParse(this.tbx_PovrsinaUtora.Text.Replace(".", currentSeparator).Replace(",", currentSeparator), out povrsinaUtora);
            double.TryParse(this.tbx_Presjek.Text.Replace(".", currentSeparator).Replace(",", currentSeparator), out presjek);

            var result = new List<ResultItem>();

            var zice = CreateZice();
            var brojeviZicaUSnopu = Enumerable.Range(1, maxBrojZica);

            foreach (var zica in zice.Get(from, to))
            {
                foreach (var brojZicaUSnopu in brojeviZicaUSnopu)
                {
                    var noviPresjek = zica.PresjekSnopa(brojZicaUSnopu);
                    string text = zica.ToString(brojZicaUSnopu);

                    TryAddResultItem(result, presjek, maxOdstupanje, slojnost, brojZavoja, povrsinaUtora, noviPresjek, text, 0);
                }
            }

            var maxRazmak = sveKombinacije ? 3 : 1;

            foreach (var zica1 in zice.Get(from, to))
            {
                foreach (var brojZica1USnopu in brojeviZicaUSnopu)
                {
                    var fromIducaZica = zica1.Order + 1;
                    var toMaximalniRazmak = zica1.Order + maxRazmak;
                    foreach (var zica2 in zice.Get(fromIducaZica, toMaximalniRazmak))
                    {
                        foreach (var brojZica2USnopu in brojeviZicaUSnopu)
                        {
                            if (brojZica1USnopu + brojZica2USnopu > maxBrojZica) continue;
                            var noviPresjek = zica1.PresjekSnopa(brojZica1USnopu) + zica2.PresjekSnopa(brojZica2USnopu);
                            var text = Zica.ToString(zica1, brojZica1USnopu, zica2, brojZica2USnopu);
                            var razmak = zica2.Order - zica1.Order;

                            TryAddResultItem(result, presjek, maxOdstupanje, slojnost, brojZavoja, povrsinaUtora, noviPresjek, text, razmak);
                        }
                    }
                }
            }


            this.grid_Rezultat.ItemsSource = result.OrderBy(x => x.Razmak).ThenBy(x => x.Odstupanje);
        }

        private static void TryAddResultItem(
            List<ResultItem> collection, 
            double presjek, 
            int maxOdstupanje, 
            int slojnost, 
            int brojZavoja, 
            double povrsinaUtora, 
            double noviPresjek, 
            string text, 
            int razmak)
        {
            var odstupanje = (Math.Abs(noviPresjek - presjek) / presjek) * 100;

            if (odstupanje < maxOdstupanje)
            {
                collection.Add(new ResultItem
                   {
                       NoviPresjek = noviPresjek,
                       Odstupanje = odstupanje,
                       Punjenje = 100 * slojnost * brojZavoja * noviPresjek / povrsinaUtora,
                       Zica = text,
                       Razmak = razmak
                   });
            }
        }

        private List<Zica> CreateZice()
        {
            int i = 0;
            return Configuration.Zice.OrderBy(x => x).Select(x => new Zica(x, i++)).ToList();
        }
    }
}
