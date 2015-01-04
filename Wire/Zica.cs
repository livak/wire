using System;

namespace Wire
{
    public class Zica
    {
        public double Presjek { get; private set; }
        public double Promjer { get; private set; }
        public int Order { get; private set; }

        public Zica(double promjer, int order) : this(promjer)
        {
            Order = order;
        }

        public Zica(double promjer)
        {
            Promjer = promjer;
            Presjek = PovrsinaKruga(promjer);
        }

        public double PresjekSnopa(int brojZica)
        {
            return brojZica * Presjek;
        }

        private double PovrsinaKruga(double promjer)
        {
            double radijus = promjer / 2;
            return Math.Pow(radijus, 2) * Math.PI;
        }

        public static string ToString(Zica zica1, int brojZica1USnopu, Zica zica2, int brojZica2USnopu)
        {
            return string.Format("{0}\t{1}",
                                zica1.ToString(brojZica1USnopu, ",-8:0.###"),
                                zica2.ToString(brojZica2USnopu, ":0.###"));
        }
        public string ToString(int brojZica, string format = "")
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                format = "{0}";
            }
            else
            {
                format = "{0" + format + "}";
            }

            return brojZica + " x " + string.Format(format, this.Promjer);
        }
    }
}
