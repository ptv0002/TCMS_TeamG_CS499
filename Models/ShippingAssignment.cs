using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("ShippingAssignment")]
    public class ShippingAssignment
    {
        public ShippingAssignment()
        {
            AssignmentDetails = new HashSet<AssignmentDetail>();
        }
        [Key]
        public int Id { get; set; }
        public string VehicleId { get; set; }
        public string EmployeeId { get; set; }
        [Display(Name = "Departure Time")]
        [Column(TypeName = "DateTime")]
        public DateTime DepartureTime { get; set; }
        [Column(TypeName = "bit")]
        public bool? Status { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual ICollection<AssignmentDetail> AssignmentDetails { get; set; }
        
    }
}
