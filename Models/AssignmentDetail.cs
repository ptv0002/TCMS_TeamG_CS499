using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("AssignmentDetail")]
    public class AssignmentDetail
    {
        public int ID { get; set; }

        public int? ShippingAssignmentlID { get; set; }

        public int? OrderInfoID { get; set; }

        public bool? InShipping { get; set; }

        public bool? ArrivalConfirm { get; set; }

        public DateTime? ArrivalTime { get; set; }

        public bool? Status { get; set; }

        public virtual OrderInfo OrderInfo { get; set; }

        public virtual ShippingAssignment ShippingAssignment { get; set; }
    }
}
