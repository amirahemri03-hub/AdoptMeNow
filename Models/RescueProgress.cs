using System.ComponentModel.DataAnnotations;

namespace AdoptMeNow.Models
{
    public class RescueProgress
    {
        public int Id { get; set; }

        [Required]
        public int RescueReportId { get; set; }

        public RescueReport RescueReport { get; set; }

        [Required]
        public string Status { get; set; }

        public string? Note { get; set; }

        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}