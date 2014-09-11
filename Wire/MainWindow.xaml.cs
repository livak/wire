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

            for (int zica = from; zica <= to; zica++)
            {
                for (int brojZica = 1; brojZica <= maxBrojZica; brojZica++)
                {
                    var noviPresjek = UkupnaPovrsina(Zice[zica], brojZica);
                    string text = brojZica + " x " + Zice[zica];
                    AddResultItem(result, presjek, maxOdstupanje, slojnost, brojZavoja, povrsinaUtora, noviPresjek, text, 0);
                }
            }

            var maxRazmak = sveKombinacije ? 3 : 1;
            for (int zica = from; zica <= to; zica++)
            {
                for (int brojZica = 1; brojZica <= maxBrojZica; brojZica++)
                {
                    for (int susjednaZica = zica + 1; susjednaZica <= zica + maxRazmak; susjednaZica++)
                    {
                        if (susjednaZica >= Zice.Count) continue;
                        for (int brojSusjednihZica = 1; brojSusjednihZica <= maxBrojZica; brojSusjednihZica++)
                        {
                            if (brojZica + brojSusjednihZica > maxBrojZica) continue;

                            var noviPresjek = UkupnaPovrsina(Zice[zica], brojZica) +
                                              UkupnaPovrsina(Zice[susjednaZica], brojSusjednihZica);

                            var text = string.Format("{0} x {1,-8:0.###}\t{2} x {3:0.###}", brojZica, Zice[zica], brojSusjednihZica, Zice[susjednaZica]);
                            AddResultItem(result, presjek, maxOdstupanje, slojnost, brojZavoja, povrsinaUtora, noviPresjek, text, susjednaZica - zica);
                        }
                    }
                }
            }

            this.grid_Rezultat.ItemsSource = result.OrderBy(x => x.Razmak).ThenBy(x => x.Odstupanje);
        }

        private static void AddResultItem(
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

        public static List<double> Zice = new List<double> 
        { 
            .2,     .224,   .25,    .28,    .3, 
            .315,   .335,   .355,   .375,   .4,     .425,   .45,    .475, 
            .5,     .56,    .6,     .63,    .65,    .67,    .71,    .75,    .8, 
            .85,    .9,     .95,    1,      1.06,   1.1,    1.12,   1.25,   1.5,   1.6,   1.7
        };
    }
 
}
