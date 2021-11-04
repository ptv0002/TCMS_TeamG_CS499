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
        public IEnumerable<Vehicle> RoutineList { get; set; }
        public StatusViewModel ShippingStatus { get; set; }
        public IEnumerable<ShippingAssignment> ShippingList { get; set; }
    }
    public class ShippingViewModel
    {
        public int Id { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string VehicleID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public DateTime DepartureTime { get; set; }
        public bool? Status { get; set; }

        public IEnumerable<AssignmentDetail> AssignmentDetails { get; set; }
    }
}
