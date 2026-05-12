using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdoptMeNow.Models;

namespace AdoptMeNow.ViewModels
{
    public class MyApplicationViewModel
    {
        public int AdoptionId { get; set; }
        public Pet Pet { get; set; }
        public string Status { get; set; }
         public string DisplayStatus =>
        Status switch
        {
            "Pending" => "In Progress",
            "Approved" => "Success",
            "Rejected" => "Rejected",
            "Cancelled" => "Cancelled",
            _ => Status
        };
        public DateTime ApplicationDate { get; set; }
        public DateTime? PickupDate { get; set; }
        public string RejectionReason { get; set; } 
    }
}