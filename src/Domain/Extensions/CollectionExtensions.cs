using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null) return true;

            if (source is IList<T> list)
            {
                return list.Count == 0;
            }

            if (source is T[] array)
            {
                return array.Length == 0;
            }

            return !source.Any();
        }

        public static bool IsNullOrEmpty<TObject, T>(this TObject obj, Func<TObject, IEnumerable<T>> propertySelector)
        {
            if (obj == null) return true;

            var collection = propertySelector(obj);
            return collection.IsNullOrEmpty();
        }
    }
}
