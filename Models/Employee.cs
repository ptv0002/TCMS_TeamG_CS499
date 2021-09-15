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
            MaintenanceInfo = new HashSet<MaintenanceInfo>();
            ShippingAssignment = new HashSet<ShippingAssignment>();
        }

        [StringLength(10)]
        public string ID { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string Position { get; set; }

        public bool? Status { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        public int? Zip { get; set; }

        [StringLength(50)]
        public string HomePhoneNum { get; set; }

        [StringLength(50)]
        public string CellPhone { get; set; }

        public double? PayRate { get; set; }

        public DateTime? StartDate { get; set; }

        public virtual ICollection<MaintenanceInfo> MaintenanceInfo { get; set; }

        public virtual ICollection<ShippingAssignment> ShippingAssignment { get; set; }
    }
}
