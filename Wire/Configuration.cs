using System.Collections.Generic;
using System.Linq;

namespace Wire
{
    public class Configuration
    {
        public static List<double> Zice()
        {
            return new List<double>
            {
                .2,     .224,   .25,    .28,    .3,
                .315,   .335,   .355,   .375,   .4,     .425,   .45,    .475,
                .5,     .56,    .6,     .63,    .65,    .67,    .71,    .75,    .8,
                .85,    .9,     .95,    1,      1.06,   1.1,    1.12,   1.25,   1.5,   1.6,   1.7
            }.OrderBy(x => x).ToList();
        }
    }
}
