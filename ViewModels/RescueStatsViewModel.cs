using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdoptMeNow.ViewModels
{
    public class RescueStatsViewModel
    {
        public int TotalReports { get; set; }
        public int Pending { get; set; }
        public int Active { get; set; }
        public int Adoption { get; set; }
        public int Closed { get; set; }
    }
}