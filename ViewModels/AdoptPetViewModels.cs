using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AdoptMeNow.Models;

namespace AdoptMeNow.ViewModels
{
    public class AdoptPetViewModels
    {
        public Pet? Pet { get; set; }   // for display (GET)

        public int PetId { get; set; } // for saving (POST)

        //Step 1
        [Required]
        public string FullName { get; set; }
        public string Email { get; set; }
        public int? Age { get; set; }
        public string Address { get; set; }
        public string IcNumber { get; set; }
        public string ContactInfo { get; set; }

        // Step 2
        public bool? HasTimeForPet { get; set; }
        public bool? IsFinanciallyReady { get; set; }
        public bool? IsEmotionallyReady { get; set; }
        public bool? WillingVetVisits { get; set; }
        public bool? HasEnoughSpace { get; set; }

        // Step 3
        public bool AgreeTerms { get; set; }

    }
}