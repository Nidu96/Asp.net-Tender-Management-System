using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenderManagementSystem.Models
{
    public class Showroom
    {
        // Creating get and set methods for Id
        public int Id { get; set; }

        // Creating get and set methods for ShowroomID
        public string ShowroomID { get; set; }

        // Creating get and set methods for ManagerID
        public string ManagerID { get; set; }

        // Creating get and set methods for LocationID
        public string LocationID { get; set; }

        // Creating get and set methods for City
        public string City { get; set; }
    }
}