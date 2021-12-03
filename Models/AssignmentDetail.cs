using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("AssignmentDetail")]
    public class AssignmentDetail
    {

        public int Id { get; set; }
        [Display(Name = "Order ID")]
        public int? OrderInfoId { get; set; }
        [Display(Name = "Incoming Shipment")]
        public bool? InShipping { get; set; }
        [Display(Name = "Arrival Confirmation")]
        public bool? ArrivalConfirm { get; set; }
        [Display(Name = "Arrival Time")]
        public DateTime? ArrivalTime { get; set; }
        public bool? Status { get; set; }
        public int? ShippingAssignmentId { get; set; }
        public string Notes { get; set; }
        public string DocName { get; set; }
        public string DocType { get; set; }
        [NotMapped]
        public IFormFile Doc { get; set; }
        public byte[] DocData { get; set; }

        public virtual OrderInfo OrderInfo { get; set; }
        public virtual ShippingAssignment ShippingAssignment { get; set; }
    }
}
