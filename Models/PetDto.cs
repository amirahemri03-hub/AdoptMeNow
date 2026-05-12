using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdoptMeNow.Models
{
    public class PetDto
    
    {
        public int PetId { get; set; }
        
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public string Breed { get; set; } = "";
        [Required]
        public string Category { get; set; } = "";
        [Required]
        public int Age { get; set; } 
        [Required]
        public string Gender { get; set; } = "";
        [Required]
        public string Description { get; set; }= "";
        
        public IFormFile? ImageFile  { get; set; }
        public string?ImageUrl { get; set; }


    }
}