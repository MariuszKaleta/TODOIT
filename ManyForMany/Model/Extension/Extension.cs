using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManyForMany.Model.Extension
{
    public static class Extension
    {
        private static void Check(int start, int count)
        {
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
        }

        public static IEnumerable<TElement> TryTake<TElement>(this IQueryable<TElement> elements,int start, int count)
        {
            Check(start, count);

            var collectionCount = elements.Count() - start;

            return elements.Skip(start).Take(count < collectionCount ? count : collectionCount);

        }

        public static IEnumerable<TElement> TryTake<TElement>(this List<TElement> elements, int start, int count)
        {
            Check(start, count);
            var collectionCount = elements.Count() - start;

            return elements.Take(count < collectionCount ? count : collectionCount);
        }

        public static IEnumerable<TElement> TryTake<TElement>(this TElement[] elements, int start, int count)
        {
            Check(start,count);
            var collectionCount = elements.Count() - start;

            return elements.Take(count < collectionCount ? count : collectionCount);
        }

        public static IEnumerable<TElement> TryTake<TElement>(this IEnumerable<TElement> elements, int start, int count)
        {
            Check(start,count);

            var i = 0;

            using (var enumerator = elements.Skip(start).GetEnumerator())
            {
                while (enumerator.MoveNext() && i++< count)
                {
                    yield return enumerator.Current;
                }
            }
        }
    }
}
