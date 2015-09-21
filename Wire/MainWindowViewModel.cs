using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Wire.Infrastructure;
using Wire.Properties;

namespace Wire
{
    [DataContract]
    public class MainWindowViewModel : ViewModeBase
    {
        private static Settings settings = Settings.Default;
        public static object Instance;

        [DataMember]
        public string BrojZavoja
        {
            get { return settings.BrojZavoja; }
            set { settings.BrojZavoja = value; OnPropertyChanged(); }
        }
        [DataMember]
        public string MaxOdstupanje
        {
            get { return settings.MaxOdstupanje; }
            set { settings.MaxOdstupanje = value; OnPropertyChanged(); }
        }
        [DataMember]
        public string MaxBrojZica
        {
            get { return settings.MaxBrojZica; }
            set { settings.MaxBrojZica = value; OnPropertyChanged(); }
        }
        [DataMember]
        public string PovrsinaUtora
        {
            get { return settings.PovrsinaUtora; }
            set { settings.PovrsinaUtora = value; OnPropertyChanged(); }
        }
        [DataMember]
        public string Presjek
        {
            get { return settings.Presjek; }
            set { settings.Presjek = value; OnPropertyChanged(); }
        }
        [DataMember]
        public bool Slojnost
        {
            get { return settings.Slojnost; }
            set { settings.Slojnost = value; OnPropertyChanged(); }
        }
        [DataMember]
        public bool Slojnost2
        {
            get { return settings.Slojnost2; }
            set { settings.Slojnost2 = value; OnPropertyChanged(); }
        }
        [DataMember]
        public int From
        {
            get { return settings.From; }
            set { settings.From = value; OnPropertyChanged(); }
        }
        [DataMember]
        public int To
        {
            get { return settings.To; }
            set { settings.To = value; OnPropertyChanged(); }
        }
        [DataMember]
        public bool SveKombinacije
        {
            get { return settings.SveKombinacije; }
            set { settings.SveKombinacije = value; OnPropertyChanged(); }
        }
        [DataMember]
        public string NemaZice
        {
            get { return settings.NemaZice; }
            set { settings.NemaZice = value; OnPropertyChanged(); }
        }
        [DataMember]
        public string BrojZica1
        {
            get { return settings.BrojZica1; }
            set { settings.BrojZica1 = value; OnPropertyChanged(); }
        }
        [DataMember]
        public string PromjerZice1
        {
            get { return settings.PromjerZice1; }
            set { settings.PromjerZice1 = value; OnPropertyChanged(); }
        }
        [DataMember]
        public string BrojZica2
        {
            get { return settings.BrojZica2; }
            set { settings.BrojZica2 = value; OnPropertyChanged(); }
        }
        [DataMember]
        public string PromjerZice2
        {
            get { return settings.PromjerZice2; }
            set { settings.PromjerZice2 = value; OnPropertyChanged(); }
        }

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

        private RelayCommand doCalculationCommand;

        public RelayCommand DoCalculationCommand
        {
            get
            {   
                if (doCalculationCommand == null)
                {
                    doCalculationCommand = new RelayCommand(() => DoCalculation());
                }
                return doCalculationCommand;
            }
        }

        private RelayCommand undoCommand;

        public RelayCommand UndoCommand
        {
            get
            {
                if (undoCommand == null)
                {
                    undoCommand = new RelayCommand(DoUndo, CanUndo);
                }
                return undoCommand;
            }
        }

        private RelayCommand redoCommand;

        public RelayCommand RedoCommand
        {
            get
            {
                if (redoCommand == null)
                {
                    redoCommand = new RelayCommand(DoRedo, CanRedo);
                }
                return redoCommand;
            }
        }

        private RelayCommand<double?> removeZicaCommand;

        public RelayCommand<double?> RemoveZicaCommand
        {
            get
            {
                if (removeZicaCommand == null)
                {
                    removeZicaCommand = new RelayCommand<double?>(zica => { DoRemoveZica(zica); });
                }
                return removeZicaCommand;
            }
        }

        private RelayCommand<string> addZicaCommand;

        public RelayCommand<string> AddZicaCommand
        {
            get
            {
                if (addZicaCommand == null)
                {
                    addZicaCommand = new RelayCommand<string>(zica => { DoAddZica(zica); });
                }
                return addZicaCommand;
            }
        }

        private RelayCommand resetAllCommand;

        public RelayCommand ResetAllCommand
        {
            get
            {
                if (resetAllCommand == null)
                {
                    resetAllCommand = new RelayCommand(DoResetAll);
                }
                return resetAllCommand;
            }
        }

        private RelayCommand setPresjekCommand;

        public RelayCommand SetPresjekCommand
        {
            get
            {
                if (setPresjekCommand == null)
                {
                    setPresjekCommand = new RelayCommand(SetPresjekFromExistingWires);
                }
                return setPresjekCommand;
            }
        }


        public MainWindowViewModel()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            Zice = new ObservableCollection<double>(Configuration.Zice());

            DoCalculation();
        }

        public void DoUndo()
        {
            Undo();
            RefreshUndoRedoCanExecute();
            DoCalculation(saveState: false);
        }

        public void DoRedo()
        {
            Redo();
            RefreshUndoRedoCanExecute();
            DoCalculation(saveState: false);
        }

        private void RefreshUndoRedoCanExecute()
        {
            RedoCommand.RaiseCanExecuteChanged();
            UndoCommand.RaiseCanExecuteChanged();
        }

        public void DoCalculation(bool saveState = true)
        {
            if (saveState)
            {
                this.SaveState();
                RefreshUndoRedoCanExecute();
            }

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
            var calculator = new WireCalculator(promjeriZica: Zice.Get(From, To), nemaZicaPromjeri: nemaZica);

            Result = calculator
                .GetResults(inputParams)
                .OrderBy(x => x.Razmak)
                .ThenBy(x => x.Odstupanje);
        }

        public void DoRemoveZica(double? zica)
        {
            if (!zica.HasValue) return;
            Zice.Remove(zica.Value);
            SaveZice();
        }

        private void DoAddZica(string zica)
        {
            double zicaAsDouble = ParseDouble(zica);
            if (Zice.Contains(zicaAsDouble)) return;
            Zice.AddInOrder(zicaAsDouble);
            SaveZice();
        }

        private void DoResetAll()
        {
            Zice = new ObservableCollection<double>(Configuration.GetDefaults());
            Settings.Default.Reset();
            NotifyAllPropertiesChanged();
            SaveZice();
        }
        private void SaveZice()
        {
            Configuration.SaveZice(Zice);
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
