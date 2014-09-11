using System;
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

        private IEnumerable<ResultItem> JednaVrstaZica(InputParams @params, IEnumerable<int> brojeviZicaUSnopu)
        {
            foreach (var zica in zice)
            {
                foreach (var brojZicaUSnopu in brojeviZicaUSnopu)
                {
                    ResultItem resultItem;
                    bool success = TryCreateResultItem(
                        resultItem: out resultItem,
                        @params: @params,
                        noviPresjek: zica.PresjekSnopa(brojZicaUSnopu),
                        text: zica.ToString(brojZicaUSnopu),
                        razmak: 0);

                    if (success) yield return resultItem;
                }
            }
        }

        private IEnumerable<ResultItem> KonbinacijaDvijeVrsteZice(InputParams @params, IEnumerable<int> brojeviZicaUSnopu)
        {
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

                            ResultItem resultItem;
                            bool success = TryCreateResultItem(
                                resultItem: out resultItem,
                                @params: @params,
                                noviPresjek: zica1.PresjekSnopa(brojZica1USnopu) + zica2.PresjekSnopa(brojZica2USnopu),
                                text: Zica.ToString(zica1, brojZica1USnopu, zica2, brojZica2USnopu),
                                razmak: zica2.Order - zica1.Order);

                            if (success) yield return resultItem;
                        }
                    }
                }
            }
        }

        public static bool TryCreateResultItem(out ResultItem resultItem, InputParams @params, double noviPresjek, string text, int razmak)
        {
            resultItem = null;
            var odstupanje = (Math.Abs(noviPresjek - @params.Presjek) / @params.Presjek) * 100;
            bool success = odstupanje < @params.MaxOdstupanje;
            if (success)
            {
                resultItem = new ResultItem
                {
                    NoviPresjek = noviPresjek,
                    Odstupanje = odstupanje,
                    Punjenje = 100 * @params.Slojnost * @params.BrojZavoja * noviPresjek / @params.PovrsinaUtora,
                    Zica = text,
                    Razmak = razmak
                };
            }
            return success;
        }

        private static List<Zica> CreateZice(IEnumerable<double> promjeriZica)
        {
            int i = 0;
            return promjeriZica.OrderBy(x => x).Select(x => new Zica(x, i++)).ToList();
        }
    }
}
