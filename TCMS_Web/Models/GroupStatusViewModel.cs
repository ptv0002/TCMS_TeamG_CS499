using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class MaintenanceMonthlyReportViewModel
    {
        public IEnumerable<MaintenanceInfo> InfoList { get; set; }
        public IEnumerable<MaintenanceDetail> DetailList { get; set; }
    }
    public class ShippingViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Vehicle ID")]
        public string VehicleID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        [Display(Name = "Departure Time")]
        public DateTime DepartureTime { get; set; }
        public bool? Status { get; set; }

        public IEnumerable<AssignmentDetail> AssignmentDetails { get; set; }
    }

    public class MaintenanceViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Vehicle ID")]
        public string VehicleID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        [Display(Name = "Departure Time")]
        public DateTime? DateTime { get; set; }
        public bool? Status { get; set; }
        public string Notes { get; set; }
        public IEnumerable<MaintenanceDetail> MaintenanceDetails { get; set; }
    }
}
