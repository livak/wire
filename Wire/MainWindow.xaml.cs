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
        }

        private double[,] CreateMatrica(int maxBrojZica)
        {
            double[,] matrica = new double[Zice.Count, maxBrojZica + 1];
            for (int i = 0; i < Zice.Count; i++)
            {
                matrica[i, 0] = Zice[i];
                for (int j = 1; j < matrica.GetLength(1); j++)
                {
                    matrica[i, j] = Math.Pow(Zice[i] / 2, 2) * Math.PI * j;
                }
            }

            return matrica;
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

            double[,] Matrica = CreateMatrica(maxBrojZica);

            var result = new List<ResultItem>();

            for (int i = from; i <= to; i++)
            {
                for (int j = 1; j < Matrica.GetLength(1); j++)
                {
                    var noviPresjek = Matrica[i, j];
                    string text = j + " x " + Matrica[i, 0];
                    AddResultItem(result, presjek, maxOdstupanje, slojnost, brojZavoja, povrsinaUtora, noviPresjek, text, 0);
                }
            }

            var maxRazmak = sveKombinacije ? 3 : 1;
            for (int i = from; i <= to; i++)
            {
                for (int j = 1; j < Matrica.GetLength(1); j++)
                {
                    for (int k = i + 1; k <= i + maxRazmak; k++)
                    {
                        if (k >= Matrica.GetLength(0)) continue;
                        for (int l = 1; l < Matrica.GetLength(1); l++)
                        {
                            if (j + l > maxBrojZica) continue;

                            var noviPresjek = Matrica[i, j] + Matrica[k, l];
                            var text = j + " x " + Matrica[i, 0] + "      " + l + " x " + Matrica[k, 0];
                            AddResultItem(result, presjek, maxOdstupanje, slojnost, brojZavoja, povrsinaUtora, noviPresjek, text, k - i);
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
