using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace JN.ApiDemo.Utils.Sorting
{
    public static class SortingIQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderByPropertyName, string direction,
            Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }

            if (string.IsNullOrWhiteSpace(orderByPropertyName))
            {
                return source;
            }

            var orderByString = string.Empty;

            var trimmedOrderBy = orderByPropertyName.Trim();

            var isDescending = direction.Equals("desc");

            if (!mappingDictionary.ContainsKey(trimmedOrderBy))
            {
                throw new ArgumentException($"Key mapping for {trimmedOrderBy} is missing");
            }
            var propertyMappingValue = mappingDictionary[trimmedOrderBy];

            foreach (var destinationProperty in propertyMappingValue.DestinationProperties)
            {
                // revert sort order if necessary
                if (propertyMappingValue.Revert)
                {
                    isDescending = !isDescending;
                }

                orderByString = orderByString +
                                (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ")
                                + destinationProperty
                                + (isDescending ? " descending" : " ascending");
            }



            return source.OrderBy(orderByString);
        }
    }
}