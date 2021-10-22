using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace Models
{
    [Table("Employee")]
    public class Employee : IdentityUser
    {
        public Employee()
        {
            MaintenanceInfos = new HashSet<MaintenanceInfo>();
            ShippingAssignments = new HashSet<ShippingAssignment>();
        }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Position { get; set; }
        public bool? Status { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int? Zip { get; set; }
        [Display(Name = "Home Phone Number")]
        public string HomePhoneNum { get; set; }
        [Display(Name = "Pay Rate (Yearly)")]
        public double? PayRate { get; set; }
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        public virtual ICollection<MaintenanceInfo> MaintenanceInfos { get; set; }

        public virtual ICollection<ShippingAssignment> ShippingAssignments { get; set; }
    }
}
