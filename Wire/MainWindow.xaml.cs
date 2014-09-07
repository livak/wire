﻿using System;
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
            InitMatrica();
        }

        private void InitMatrica()
        {
            for (int i = 0; i < Zice.Count; i++)
            {
                Matrica[i, 0] = Zice[i];
                for (int j = 1; j < MaxBrojZicaUSnopu; j++)
                {
                    Matrica[i, j] = Math.Pow(Zice[i] / 2, 2) * Math.PI * j;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int brojZavoja;
            int maxOdstupanje;
            double povrsinaUtora;
            double presjek;
            int slojnost = radio_Slojnost1.IsChecked.Value ? 1 : 2;

            int from = drop_From.SelectedIndex;
            int to = drop_To.SelectedIndex;

            bool sveKombinacije = cbx_PrikaziSve.IsChecked ?? false;

            var currentSeparator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            int.TryParse(this.tbx_BrojZavoja.Text, out brojZavoja);
            int.TryParse(this.tbx_MaxOdstupanje.Text, out maxOdstupanje);
            double.TryParse(this.tbx_PovrsinaUtora.Text.Replace(".", currentSeparator).Replace(",", currentSeparator), out povrsinaUtora);
            double.TryParse(this.tbx_Presjek.Text.Replace(".", currentSeparator).Replace(",", currentSeparator), out presjek);

            var result = new List<ResultItem>();

            for (int i = from; i <= to; i++)
            {
                for (int j = 1; j < MaxBrojZicaUSnopu; j++)
                {
                    var noviPresjek = Matrica[i, j];
                    string text = j + " x " + Matrica[i, 0];
                    AddResultItem(result, presjek, maxOdstupanje, slojnost, brojZavoja, povrsinaUtora, noviPresjek, text, 0);
                }
            }

            var maxRazmak = sveKombinacije ? 3 : 1; 
            for (int i = from; i <= to - maxRazmak; i++)
            {
                for (int j = 1; j < MaxBrojZicaUSnopu; j++)
                {
                    for (int k = i + 1; k < i + maxRazmak + 1; k++)
                    {
                        for (int l = 1; l < MaxBrojZicaUSnopu; l++)
                        {
                            var noviPresjek = Matrica[i, j] + Matrica[k, l];
                            var text = j + " x " + Matrica[i, 0] + "      " + l + " x " + Matrica[k, 0];
                            AddResultItem(result, presjek, maxOdstupanje, slojnost, brojZavoja, povrsinaUtora, noviPresjek, text, k-i);
                        }
                    }
                }
            }

            this.grid_Rezultat.ItemsSource = result.OrderBy(x => x.Razmak).ThenBy(x => x.Odstupanje);
        }

        private static void AddResultItem(List<ResultItem> collection, double presjek, int maxOdstupanje, int slojnost, int brojZavoja, double povrsinaUtora, double noviPresjek, string text, int razmak)
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
            .14,    .16,    .18,    .2,     .224,   .25,    .28,    .3, 
            .315,   .335,   .355,   .375,   .4,     .425,   .45,    .475, 
            .5,     .56,    .6,     .63,    .65,    .71,    .75,    .8, 
            .85,    .9,     .95,    1,      1.06,   1.12,   1.25 
        };
        const int MaxBrojZicaUSnopu = 11;
        double[,] Matrica = new double[Zice.Count, MaxBrojZicaUSnopu];
    }


    public class ResultToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int quantity = (int)value;


                    byte n = (byte)((255 / Math.Pow(quantity + 1,0.2)));

                    return new SolidColorBrush(Color.FromRgb(n,n,n));
            }

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }  
}
