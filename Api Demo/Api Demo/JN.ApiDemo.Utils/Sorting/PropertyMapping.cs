using System;
using System.Collections.Generic;

namespace JN.ApiDemo.Utils.Sorting
{
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        public string TSourceName { get; private set; }
        public string TDestinationName { get; private set; }

        public Dictionary<string, PropertyMappingValue> MappingDictionary { get; private set; }

        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            TSourceName = typeof(TSource).Name;
            TDestinationName = typeof(TDestination).Name;

            MappingDictionary = mappingDictionary ??
                                 throw new ArgumentNullException(nameof(mappingDictionary));
        }
    }
}