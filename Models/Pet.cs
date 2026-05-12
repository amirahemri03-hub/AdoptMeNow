using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace AdoptMeNow.Models
{
    public class Pet
    {
        public int PetId { get; set; }

        public string Name { get; set; }

        public string Breed { get; set; }
        public string Category { get; set; }

        public int Age { get; set; }

        public string Gender { get; set; }

        public string Description { get; set; }

        // Optional: track availability
        //public bool IsAdopted { get; set; } = false;

        public string?ImageUrl { get; set; }

        public string Status { get; set; } = "Available";
    }
 }
