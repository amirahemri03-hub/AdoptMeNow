using AdoptMeNow.Models;

namespace AdoptMeNow.ViewModels
{
    public class PetViewModel
    {
        public Pet Pet { get; set; }
        public bool HasApplied { get; set; }
        public bool HasPendingApplication { get; set; }
        public string? ApplicantUserId { get; set; }
        public string? ApplicationStatus { get; set; }
        public bool IsSaved { get; set; }

    }
}