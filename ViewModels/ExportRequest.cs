using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdoptMeNow.ViewModels
{
    public class ExportRequest
    {
        //public string ReportType { get; set;} 

        public string Format { get; set; } //pdf/excel
        public  DateTime? FromDate { get; set;}
        public DateTime? ToDate { get; set;}
    }
}