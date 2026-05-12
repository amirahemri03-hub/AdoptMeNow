using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace AdoptMeNow.Models
{
    public class SavedPets
    {
        [Key]
        public int Id {get; set;}
        public string UserId {get; set;}
        public int PetId {get; set;}

        public Users User { get; set;}
        public Pet Pet {get; set;}

    }
}