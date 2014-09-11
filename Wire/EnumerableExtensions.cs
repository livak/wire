using System;
using System.Collections.Generic;

namespace Wire
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> Get<TSource>(this IList<TSource> source, int fromIndex, int toIndex)
        {
            int currIndex = fromIndex;
            int maxIndex = source.Count - 1;
            while (currIndex <= Math.Min(toIndex, maxIndex))
            {
                yield return source[currIndex];
                currIndex++;
            }
        }

        public static void TryAddResultItem(this IList<ResultItem> target, InputParams @params, double noviPresjek, string text, int razmak)
        {
            var odstupanje = (Math.Abs(noviPresjek - @params.Presjek) / @params.Presjek) * 100;

            if (odstupanje < @params.MaxOdstupanje)
            {
                target.Add(new ResultItem
                {
                    NoviPresjek = noviPresjek,
                    Odstupanje = odstupanje,
                    Punjenje = 100 * @params.Slojnost * @params.BrojZavoja * noviPresjek / @params.PovrsinaUtora,
                    Zica = text,
                    Razmak = razmak
                });
            }
        }
    } 
}
