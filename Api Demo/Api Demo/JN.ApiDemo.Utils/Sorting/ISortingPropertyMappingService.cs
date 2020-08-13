using System.Collections.Generic;

namespace JN.ApiDemo.Utils.Sorting
{
    public interface ISortingPropertyMappingService
    {
        bool ValidMappingExistsFor<TSource, TDestination>(string propertyName);

        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        Dictionary<string, PropertyMappingValue> GetPropertyMappingByTypeNames(string tSourceName, string tDestinationName);
    }
}