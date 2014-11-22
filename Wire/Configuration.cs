using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Wire.Properties;

namespace Wire
{
    public class Configuration
    {
        public static List<double> GetDefaults()
        {
            return new List<double>
                {
                    .2,     .224,   .25,    .28,    .3,
                    .315,   .335,   .355,   .375,   .4,     .425,   .45,    .475,
                    .5,     .56,    .6,     .63,    .65,    .67,    .71,    .75,    .8,
                    .85,    .9,     .95,    1,      1.06,   1.1,    1.12,   1.25,   1.5,   1.6,   1.7
                }.OrderBy(x => x).ToList();
        }
        public static List<double> Zice()
        {
            return (Settings.Default.Zice == null || (Settings.Default.Zice.Count == 0)) ?
                GetDefaults() :
                Settings.Default.Zice.Cast<string>().Select(x => double.Parse(x, CultureInfo.InvariantCulture)).OrderBy(x => x).ToList();
        }

        public static void SaveZice(IEnumerable<double> zice)
        {
            Settings.Default.Zice = new StringCollection();
            Settings.Default.Zice.AddRange(zice.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray());
            Settings.Default.Save();
        }
    }
}
