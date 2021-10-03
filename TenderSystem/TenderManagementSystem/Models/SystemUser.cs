using System;
using System.Collections.Generic;
using System.Linq;

namespace TenderManagementSystem.Models
{
    public class SystemUser
    {
        // Creating get and set methods for Id
        public int Id { get; set; }

        // Creating get and set methods for UserID
        public string UserID { get; set; }

        // Creating get and set methods for Name
        public string FullName { get; set; }

        // Creating get and set methods for Phone
        public string Phone { get; set; }

        // Creating get and set methods for AddressLine_1
        public string AddressLine_1 { get; set; }

        // Creating get and set methods for AddressLine_2
        public string AddressLine_2 { get; set; }

        // Creating get and set methods for AddressLine_3
        public string AddressLine_3 { get; set; }

        // Creating get and set methods for Email
        public string Email { get; set; }

        // Creating get and set methods for Confirm Email
        public string ConfirmEmail { get; set; }

        // Creating get and set methods for Password
        public string Password { get; set; }

        // Creating get and set methods for Confirm Password
        public string ConfirmPassword { get; set; }

        // Creating get and set methods for UserRole
        public string UserRole { get; set; }

        // Creating get and set methods for Error
        public string Error { get; set; }
    }
}