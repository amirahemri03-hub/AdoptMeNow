using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace AdoptMeNow.ViewModels
{
    public class RescueStatusUpdateViewModel
    {
        [Required]
        public int RescueReportId { get; set; }

        [Required]
        public string Status { get; set; }

        public string? StatusNote { get; set; }

        public IFormFile? StatusImageFile { get; set; }
    }
}