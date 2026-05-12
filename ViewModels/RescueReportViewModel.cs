using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AdoptMeNow.ViewModels
{
    public class RescueReportViewModel
    {
        [Required]
        public IFormFile ImageFile { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Location { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}