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
            AssignmentDetails = new HashSet<AssignmentDetail>();
        }

        public int Id { get; set; }
        public int? SourceId { get; set; }
        public int? DestinationId { get; set; }
        public string SourceAddress { get; set; }
        public string DestinationAddresss { get; set; }
        public bool? Status { get; set; }
        public string DocName { get; set; }
        public string DocType { get; set; }
        public byte[] DocData { get; set; }
        public bool? SourcePay { get; set; }
        public bool? PayStatus { get; set; }
        public double? TotalOrder { get; set; }
        public double? ShippingFee { get; set; }
        public DateTime? EstimateArrivalTime { get; set; }

        public Company Destination { get; set; }
        [InverseProperty("OrderInfoSources")]
        public Company Source { get; set; }
        public virtual ICollection<AssignmentDetail> AssignmentDetails { get; set; }
    }
}
