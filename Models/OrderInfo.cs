using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    [Table("OrderInfo")]
    public class OrderInfo
    {
        public OrderInfo()
        {
            AssignmentDetail = new HashSet<AssignmentDetail>();
        }

        public int ID { get; set; }

        public int? SourceID { get; set; }

        public int? DestinationID { get; set; }

        [StringLength(50)]
        public string SourceAddress { get; set; }

        [StringLength(50)]
        public string DestinationAddresss { get; set; }

        public bool? Status { get; set; }

        [StringLength(100)]
        public string DocName { get; set; }

        [StringLength(100)]
        public string DocType { get; set; }

        public byte[] DocData { get; set; }

        public bool? SourcePay { get; set; }

        public bool? PayStatus { get; set; }

        public double? TotalOrder { get; set; }

        public double? ShippingFee { get; set; }

        public DateTime? EstimateArrivalTime { get; set; }

        public virtual ICollection<AssignmentDetail> AssignmentDetail { get; set; }

        public virtual Company Company { get; set; }

        public virtual Company Company1 { get; set; }
    }
}
