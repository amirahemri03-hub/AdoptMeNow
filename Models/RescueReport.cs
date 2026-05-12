using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AdoptMeNow.Models
{
    public class RescueReport
    {
        public int Id { get; set; }

        // FK 
        [Required]
        public string UserId { get; set; }

        public Users User { get; set; }

        [Required]
        public string ImagePath { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Location { get; set; }

        // Map coordinates 
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        //Rescue Status
        public string Status { get; set; } = "Pending";
        
        public DateTime ReportedAt { get; set; } = DateTime.Now;
        public string? RejectReason { get; set; }
        public string? CancelReason { get; set; }
        public ICollection<RescueProgress> ProgressUpdates { get; set; } = new List<RescueProgress>();
    }
}
