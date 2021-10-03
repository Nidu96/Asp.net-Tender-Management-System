using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TenderManagementSystem.Models
{
    public class Bid
    {
        // Creating get and set methods for Id
        public int Id { get; set; }

        // Creating get and set methods for BidID
        public string BidID { get; set; }

        // Creating get and set methods for DateSubmitted
        public string DateSubmitted { get; set; }

        // Creating get and set methods for TenderID
        public string TenderID { get; set; }

        // Creating get and set methods for TenderName
        public string TenderName { get; set; }

        // Creating get and set methods for TenderDate
        public string TenderDate { get; set; }

        // Creating get and set methods for UserID
        public string UserID { get; set; }

        // Creating get and set methods for Email
        public string Email { get; set; }

        // Creating get and set methods for Price
        public string Price { get; set; }
        // Creating get and set methods for Description
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        // Creating get and set methods for ImageURL
        public string ImageURL { get; set; }

        // Creating get and set methods for Status
        public string Status { get; set; }
    }
}