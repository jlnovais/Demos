using System.Collections.Generic;
using JN.ApiDemo.Identity.Domain;

namespace JN.ApiDemo.Identity.Dto
{
    public class UserWithRoles
    {
        public ApplicationUser User { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}