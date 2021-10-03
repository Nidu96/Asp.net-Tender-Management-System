using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TenderManagementSystem.Models
{
    public class Tender
    {
        // Creating get and set methods for Id
        public int Id { get; set; }

        // Creating get and set methods for TenderID
        public string TenderID { get; set; }

        // Creating get and set methods for TenderName
        public string TenderName { get; set; }

        // Creating get and set methods for Description
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        // Creating get and set methods for TenderDate
        public string TenderDate { get; set; }
        // Creating get and set methods for BidsList
        public List<Bid> BidsList { get; set; }

        // Creating get and set methods for Image
        public System.Web.HttpPostedFileBase Image { get; set; }
        public string ImageURL { get; set; }

        // Creating get and set methods for UserRole
        public string UserRole { get; set; }

    }
}