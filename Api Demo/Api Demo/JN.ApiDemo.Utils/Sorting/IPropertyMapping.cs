using System.Collections.Generic;

namespace JN.ApiDemo.Utils.Sorting
{
    public interface IPropertyMapping
    {
        //string TSourceName { get; }
        //string TDestinationName { get; }
        string TSourceName { get; }
        string TDestinationName { get; }
        Dictionary<string, PropertyMappingValue> MappingDictionary { get; }
    }
}
