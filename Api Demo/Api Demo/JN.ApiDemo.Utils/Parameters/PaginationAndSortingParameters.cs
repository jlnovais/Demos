namespace JN.ApiDemo.Utils.Parameters
{
    /// <summary>
    /// Pagination and sorting parameters
    /// </summary>
    public class PaginationAndSortingParameters
    {
        private const int MaxPageSize = 20;

        private int _page = 1;
        
        /// <summary>
        /// page to return
        /// </summary>
        public int Page
        {
            get => _page;
            set => _page = (value < 1) ? 1 : value;
        }

        private int _pageSize = 10;

        /// <summary>
        /// number of items per page
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize || value < 1) ? MaxPageSize : value;
        }

        /// <summary>
        /// field name used to order the results
        /// </summary>
        public string OrderBy { get; set; } = "";
        /// <summary>
        /// sort direction - asc or desc
        /// </summary>
        public string Direction { get; set; } = "asc";

        protected string SortingMappingSourceTypeName { get; set; }
        protected string SortingMappingDestinationTypeName { get; set; }

        public void SetSortingMappingSourceTypeName(string name)
        {
            SortingMappingSourceTypeName = name;
        }
        public void SetSortingMappingDestinationTypeName(string name)
        {
            SortingMappingDestinationTypeName = name;
        }
        public string GetSortingMappingSourceTypeName()
        {
            return SortingMappingSourceTypeName;
        }

        public string GetSortingMappingDestinationTypeName()
        {
            return SortingMappingDestinationTypeName;
        }

    }
}
