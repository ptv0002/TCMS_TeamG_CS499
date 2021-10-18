using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMS_Web.Models
{
    public class GroupStatusViewModel<T>
    {
        public StatusViewModel StatusViewModel { get; set; }
        public IEnumerable<T> ClassModel { get; set; }
    }
    public class StatusViewModel
    {
        public string SelectedValue { get; set; }
        public Dictionary<string, string> KeyValues { get; set; }
    }
    public class HomeIndexViewModel
    {
        public StatusViewModel MaintenanceStatus { get; set; }
        public IEnumerable<MaintenanceInfo> MaintenanceList { get; set; }
        public StatusViewModel ShippingStatus { get; set; }
        public IEnumerable<ShippingAssignment> ShippingList { get; set; }
    }
}
