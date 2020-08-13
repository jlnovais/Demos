using System;
using System.Collections.Generic;
using System.Linq;

namespace JN.ApiDemo.Utils.Sorting
{
    public class SortingPropertyMappingService : ISortingPropertyMappingService
    {
        private readonly List<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public SortingPropertyMappingService(IEnumerable<IPropertyMapping> maps)
        {
            if(maps!=null)
                _propertyMappings.AddRange(maps);
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string propertyName)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return true;
            }

            return propertyMapping.ContainsKey(propertyName);
        }


        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _propertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First().MappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                                $"for <{typeof(TSource)},{typeof(TDestination)}>");
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMappingByTypeNames(string tSourceName, string tDestinationName)
        {
            if(string.IsNullOrWhiteSpace(tSourceName) || string.IsNullOrWhiteSpace(tDestinationName))
                throw new ArgumentException("Invalid TypeSourceName or TypeDestinationName");

            var matchingMapping = _propertyMappings.Where(x =>
                x.TSourceName.Equals(tSourceName) && x.TDestinationName.Equals(tDestinationName));
    
            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First().MappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                                $"for <{tSourceName},{tDestinationName}>");
        }
    }
}