using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdoptMeNow.ViewModels
{
    public class AdoptionProgressViewModels
    {
        public int AdoptionId { get; set; }

        public string FullName { get; set; }
        public string ContactInfo { get; set; }

        public string PetName { get; set; }
        public int? Age { get; set; }

        public string Address { get; set; }
        public string IcNumber { get; set; }

        public DateTime ApplicationDate { get; set; }

        public DateTime? PickupDate { get; set; } 

        public string Status { get; set; }
    }
}