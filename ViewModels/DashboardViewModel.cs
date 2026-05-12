using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdoptMeNow.Models;

namespace AdoptMeNow.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalPets { get; set; }
        public int AvailablePets { get; set; }
        public int AdoptedPets { get; set; }
        public int PendingApplications { get; set; }
        public int TotalAdoptions { get; set; }
        public List<RescueReport> RescueReports { get; set; }
        public int TotalRescueReports { get; set; }
        //public int AdoptionsThisMonth { get; set; }
        public List<MonthlyData> MonthlyAdoptions { get; set; }
    }

    public class MonthlyData
    {
        public string Label { get; set; }
        public int Total { get; set; }
    }
}