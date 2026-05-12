using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdoptMeNow.Models;


namespace AdoptMeNow.ViewModels
{
    public class HomeViewModel
    {
        public List<PetViewModel> Pets { get; set; }
        public int TotalPets { get; set; }
        public int AvailablePets { get; set; }
        public int AdoptedPets { get; set;}

    }
}