using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdoptMeNow.ViewModels
{
    public class AdoptionDetailViewModel
    {
        public int AdoptionId { get; set; }

        // Application Info
        public string FullName { get; set; }
        public string ContactInfo { get; set; }
        public string PetName { get; set; }
        public int? Age { get; set; }
        public string Address { get; set; }
        public string IcNumber { get; set; }
        public DateTime ApplicationDate { get; set; }

        // Assessment Answers
        public bool HasTimeForPet { get; set; }
        public bool IsFinanciallyReady { get; set; }
        public bool IsEmotionallyReady { get; set; }
        public bool WillingVetVisits { get; set; }
        public bool HasEnoughSpace { get; set; }


        // Management
        public string Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? PickupDate { get; set; }
    }
}