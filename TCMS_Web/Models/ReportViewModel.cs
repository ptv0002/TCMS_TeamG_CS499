using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMS_Web.Models
{
    public enum Months
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }
    public class ReportViewModel
    {
        
        public string ControllerName { get; set;}
        public string Id { get; set; }
        public Months Months { get; set; }
        public int SelectedMonth { get; set; }
        public bool IsIncoming_Individual { get; set; }
    }
}
