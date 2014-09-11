using System.Collections.Generic;
using System.Linq;

namespace Wire
{
    class WireCalculator
    {
        private List<Zica> zice;
        public WireCalculator(IEnumerable<double> promjeriZica)
        {
            zice = CreateZice(promjeriZica);
        }

        public List<ResultItem> GetResults(InputParams @params)
        {
            var brojeviZicaUSnopu = Enumerable.Range(1, @params.MaxBrojZica);

            var resultJednaVrstaZice = JednaVrstaZica(@params,brojeviZicaUSnopu);
            var rasultKonbinacijaDvijeVrsteZice = KonbinacijaDvijeVrsteZice(@params,brojeviZicaUSnopu);

            var result = new List<ResultItem>();
            result.AddRange(resultJednaVrstaZice);
            result.AddRange(rasultKonbinacijaDvijeVrsteZice);

            return result;
        }

        private List<ResultItem> JednaVrstaZica(InputParams @params, IEnumerable<int> brojeviZicaUSnopu)
        {
            var result = new List<ResultItem>();

            foreach (var zica in zice)
            {
                foreach (var brojZicaUSnopu in brojeviZicaUSnopu)
                {
                    var noviPresjek = zica.PresjekSnopa(brojZicaUSnopu);
                    string text = zica.ToString(brojZicaUSnopu);
                    result.TryAddResultItem(@params, noviPresjek, text, 0);
                }
            }

            return result;
        }

        private List<ResultItem> KonbinacijaDvijeVrsteZice(InputParams @params, IEnumerable<int> brojeviZicaUSnopu)
        {
            var result = new List<ResultItem>();
            foreach (var zica1 in zice)
            {
                foreach (var brojZica1USnopu in brojeviZicaUSnopu)
                {
                    var fromIducaZica = zica1.Order + 1;
                    var toMaximalniRazmak = zica1.Order + @params.MaxRazmak;
                    foreach (var zica2 in zice.Get(fromIducaZica, toMaximalniRazmak))
                    {
                        foreach (var brojZica2USnopu in brojeviZicaUSnopu)
                        {
                            if (brojZica1USnopu + brojZica2USnopu > @params.MaxBrojZica) continue;
                            var noviPresjek = zica1.PresjekSnopa(brojZica1USnopu) + zica2.PresjekSnopa(brojZica2USnopu);
                            var text = Zica.ToString(zica1, brojZica1USnopu, zica2, brojZica2USnopu);
                            var razmak = zica2.Order - zica1.Order;
                            result.TryAddResultItem(@params, noviPresjek, text, razmak);
                        }
                    }
                }
            }

            return result;
        }

        private static List<Zica> CreateZice(IEnumerable<double> promjeriZica)
        {
            int i = 0;
            return promjeriZica.OrderBy(x => x).Select(x => new Zica(x, i++)).ToList();
        }
    }
}
