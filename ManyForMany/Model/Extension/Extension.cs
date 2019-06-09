using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManyForMany.Model.Extension
{
    public static class Extension
    {
        public static IEnumerable<TElement> TryTake<TElement>(this IQueryable<TElement> elements, int count)
        {
            var collectionCount = elements.Count();

            return elements.Take(count < collectionCount ? count : collectionCount);
        }

        public static IEnumerable<TElement> TryTake<TElement>(this List<TElement> elements, int count)
        {
            var collectionCount = elements.Count();

            return elements.Take(count < collectionCount ? count : collectionCount);
        }

        public static IEnumerable<TElement> TryTake<TElement>(this TElement[] elements, int count)
        {
            var collectionCount = elements.Count();

            return elements.Take(count < collectionCount ? count : collectionCount);
        }

        public static IEnumerable<TElement> TryTake<TElement>(this IEnumerable<TElement> elements, int count)
        {
            var i = 0;

            using (var enumerator = elements.GetEnumerator())
            {
                while (enumerator.MoveNext() && i++< count)
                {
                    yield return enumerator.Current;
                }
            }
        }
    }
}
