using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    } 
}
