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
        public DateTime Month { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsIncoming { get; set; }
    }
}
