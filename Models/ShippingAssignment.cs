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
            AssignmentDetail = new HashSet<AssignmentDetail>();
        }

        public int ID { get; set; }

        [StringLength(10)]
        public string VehicleID { get; set; }

        [StringLength(10)]
        public string EmployeeID { get; set; }

        public DateTime? DepartureTime { get; set; }

        public bool? Status { get; set; }

        public virtual ICollection<AssignmentDetail> AssignmentDetail { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Vehicle Vehicle { get; set; }
    }
}
