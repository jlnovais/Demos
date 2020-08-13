using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JN.ApiDemo.Identity.Domain
{
    public class ApiKey
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int Id { get; set; }
        public string Key { get; set; }
        public DateTime CreationDate { get; set; }
        public bool Active { get; set; }
        public int UserId { get; set; }

        //define that UserId should be a ForeignKey for table Users (from asp.net Identity)
        [ForeignKey(nameof(UserId))] 
        public ApplicationUser User { get; set; } //<--- key for custom IdentityUser
    }
}
