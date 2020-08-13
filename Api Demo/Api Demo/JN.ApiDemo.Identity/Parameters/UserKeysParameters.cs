using JN.ApiDemo.Utils.Parameters;

namespace JN.ApiDemo.Identity.Parameters
{
    public class UserKeysParameters : PaginationAndSortingParameters
    {
        public string UserId { get; set; } = "";

        public UserKeysParameters()
        {
            base.OrderBy = "CreationDate";
        }

    }
}