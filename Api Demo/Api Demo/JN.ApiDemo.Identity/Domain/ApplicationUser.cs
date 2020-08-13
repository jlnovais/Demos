using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace JN.ApiDemo.Identity.Domain
{

    // extending default IdentityUser class for AspNetCore.Identity
    // IdentityUser --> normal table with string key;  IdentityUser<int> --> for using a table with int key
    // Important: Replace IdentityUser by ApplicationUser everywhere

    public class ApplicationUser : IdentityUser<int> 
    {
        public string NotificationEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public List<ApiKey> ApiKeys { get; set; }
    }
}
