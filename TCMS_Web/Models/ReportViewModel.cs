using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMS_Web.Models
{
    public class ReportViewModel
    {
        public string ControllerName { get; set;}
        public string Id { get; set; }
        public StatusViewModel Months { get; set; }
        public StatusViewModel Years { get; set; }
        public bool IsIncoming_Individual { get; set; }
    }
}
