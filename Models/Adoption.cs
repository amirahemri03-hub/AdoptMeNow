using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdoptMeNow.Models
{
    public class Adoption
    {
        public int AdoptionId { get; set; }

        public int PetId { get; set; }
        public Pet Pet { get; set; }  // all pet data

        // User Info
        public string FullName { get; set; }
        public string Email { get; set; }
        public int? Age { get; set; }
        public string Address { get; set; }
        public string IcNumber { get; set; }
        public string ContactInfo { get; set; }

        // Step 2 Info
        public bool HasTimeForPet { get; set; }
        public bool IsFinanciallyReady { get; set; }
        public bool IsEmotionallyReady { get; set; }
        public bool WillingVetVisits { get; set; }
        public bool HasEnoughSpace { get; set; }

        // Step3 
        public bool AgreeTerms { get; set; }

        //SYSTEM STATUS
        public string Status { get; set; }
        public DateTime ApplicationDate { get; set; }

        public string UserId { get; set; }

        public string? RejectionReason { get; set; }
        
        public DateTime? PickupDate { get; set; }
    }
}