/*
 * Order Info Model
 * Author: Veronica Vu
 * Date: 9/2/2021
 * Purpose: Provides Order Info model information that will be used to get 
 * Order Info information and set Order Info information in our SQL database 
 * 
 */

using Microsoft.AspNetCore.Http;
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
        [Display(Name = "Source Address")]
        public string SourceAddress { get; set; }
        [Display(Name = "Destination Address")]
        public string DestinationAddress { get; set; }
        public bool? Status { get; set; }
        public string DocName { get; set; }
        public string DocType { get; set; }
        [NotMapped]
        public IFormFile Doc { get; set; }
        public byte[] DocData { get; set; }
        [Display(Name = "Source Pay")]
        public bool? SourcePay { get; set; }
        [Display(Name = "Pay Status")]
        public bool? PayStatus { get; set; }
        [Display(Name = "Order's Value")]
        public double? TotalOrder { get; set; }
        [Display(Name = "Shipping Fee")]
        public double? ShippingFee { get; set; }
        [Display(Name = "Estimated Arrival Time")]
        public DateTime? EstimateArrivalTime { get; set; }

        public Company Destination { get; set; }
        [InverseProperty("OrderInfoSources")]
        public Company Source { get; set; }
        public virtual ICollection<AssignmentDetail> AssignmentDetails { get; set; }
    }
}
