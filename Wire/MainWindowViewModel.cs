﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Wire.Infrastructure;

namespace Wire
{
    public class MainWindowViewModel : ViewModeBase
    {
        public string BrojZavoja { get; set; }
        public string MaxOdstupanje { get; set; }
        public string MaxBrojZica { get; set; }
        public string PovrsinaUtora { get; set; }

        string _presjek;
        public string Presjek
        {
            get { return _presjek; }
            set { _presjek = value; OnPropertyChanged(); }
        }
        public bool Slojnost { get; set; }

        public int From { get; set; }
        public int To { get; set; }
        public bool SveKombinacije { get; set; }

        public string NemaZice { get; set; }

        public string BrojZica1 { get; set; }
        public string PromjerZice1 { get; set; }
        public string BrojZica2 { get; set; }
        public string PromjerZice2 { get; set; }

        public ObservableCollection<double> _zice;
        public ObservableCollection<double> Zice
        {
            get { return _zice; }
            set { _zice = value; OnPropertyChanged(); }
        }
        public IEnumerable<ResultItem> _result;
        public IEnumerable<ResultItem> Result
        {
            get { return _result; }
            set { _result = value; OnPropertyChanged(); }
        }

        public MainWindowViewModel()
        {
            Presjek = 0.7209.ToString();
            BrojZavoja = 45.ToString();
            PovrsinaUtora = 80.1.ToString();
            MaxOdstupanje = 2.ToString();
            MaxBrojZica = 10.ToString();
            Slojnost = true;
            From = 4;
            To = 29;
            Zice = new ObservableCollection<double>(Configuration.Zice());
            NemaZice = string.Empty;
        }

        public void DoCalculation()
        {
            var inputParams = new InputParams
            {
                BrojZavoja = ParseInt(BrojZavoja),
                MaxBrojZica = ParseInt(MaxBrojZica),
                MaxOdstupanje = ParseInt(MaxOdstupanje),
                MaxRazmak = SveKombinacije ? 3 : 1,
                PovrsinaUtora = ParseDouble(PovrsinaUtora),
                Presjek = ParseDouble(Presjek),
                Slojnost = Slojnost ? 1 : 2
            };

            var nemaZica = NemaZice.Split(';', ' ').Select(ParseDouble).Where(f => f != default(double));
            var calculator = new WireCalculator(promjeriZica: Zice.Get(From, To).Except(nemaZica));

            Result = calculator
                .GetResults(inputParams)
                .OrderBy(x => x.Razmak)
                .ThenBy(x => x.Odstupanje);
        }

        public void SetPresjekFromExistingWires()
        {
            Presjek = (new Zica(ParseDouble(PromjerZice1)).PresjekSnopa(ParseInt(BrojZica1)) +
                       new Zica(ParseDouble(PromjerZice2)).PresjekSnopa(ParseInt(BrojZica2)))
                      .ToString("0.####");
        }

        public static double ParseDouble(string s)
        {
            double r;
            double.TryParse(ToCurrentCultureSeparator(s ?? string.Empty).Trim(), out r);
            return r;
        }

        private int ParseInt(string s)
        {
            int r;
            int.TryParse(s, out r);
            return r;
        }

        public static string ToCurrentCultureSeparator(string s)
        {
            var currentSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            return s.Replace(".", currentSeparator).Replace(",", currentSeparator);
        }
    }
}
