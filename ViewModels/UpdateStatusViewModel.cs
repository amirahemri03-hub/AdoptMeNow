using System;

namespace AdoptMeNow.ViewModels
{
    public class UpdateStatusRequest
    {
        public int AdoptionId { get; set; }
        public string Status { get; set; }
        public DateTime? PickupDate { get; set; }
        public string RejectionReason { get; set; }
    }
}