using System;

namespace JN.ApiDemo.Contracts.V1.Admin.Responses
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// {
    ///    "id": 1,
    ///    "username": "user",
    ///    "email": "email@email.email",
    ///    "phoneNumber": "123456789",
    ///    "notificationEmail": "email@email.email",
    ///    "firstName": "Jose",
    ///    "lastName": "Test",
    ///    "active": true,
    ///    "description": "this a test user",
    ///    "dateCreated": "2020-07-08T19:57:23.5669785",
    ///    "roles": "Admin"
    /// }
    /// </example>
    public class UserDetailsResponse : UserDetailsBaseResponse, IUserDetails
    {
        public string PhoneNumber { get; set; }
        public string NotificationEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateCreated { get; set; }
        public string Roles { get; set; }
    }

}
