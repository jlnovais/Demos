using JN.ApiDemo.Utils.Parameters;

namespace JN.ApiDemo.Identity.Parameters
{
    public class UsersParameters : PaginationAndSortingParameters
    {
        public UsersParameters()
        {
            base.OrderBy = "id";
        }
        public string Role { get; set; } = "";
    }
}