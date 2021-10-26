using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    [Table("ShippingAssignment")]
    public class ShippingAssignment
    {
        public ShippingAssignment()
        {
            AssignmentDetails = new HashSet<AssignmentDetail>();
        }
        public int Id { get; set; }
        public string VehicleId { get; set; }
        public string EmployeeId { get; set; }
        [Display(Name = "Departure Time")]
        public DateTime? DepartureTime { get; set; }
        public bool? Status { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual ICollection<AssignmentDetail> AssignmentDetails { get; set; }
    }
}
