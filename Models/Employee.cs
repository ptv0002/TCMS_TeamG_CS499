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

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public bool? Status { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int? Zip { get; set; }
        public string HomePhoneNum { get; set; }
        public double? PayRate { get; set; }
        public DateTime? StartDate { get; set; }

        public virtual ICollection<MaintenanceInfo> MaintenanceInfos { get; set; }

        public virtual ICollection<ShippingAssignment> ShippingAssignments { get; set; }
    }
}
