/*
 * Vehicle Model
 * Author: Veronica Vu
 * Date: 9/2/2021
 * Purpose: Provides Vehicle model information that will be used to get 
 * Vehicle information and set Vehicle information in our SQL database 
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    [Table("Vehicle")]
    public class Vehicle
    {
        public Vehicle()
        {
            MaintenanceInfos = new HashSet<MaintenanceInfo>();
            ShippingAssignments = new HashSet<ShippingAssignment>();
        }

        public string Id { get; set; }
        public string Brand { get; set; }
        public int? Year { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        [Display(Name = "Ready Status")]
        public bool? ReadyStatus { get; set; }
        public bool? Status { get; set; }
        public string Parts { get; set; }
        [Display(Name = "Last Maintenance Date")]
        public DateTime? LastMaintenanceDate { get; set; }
        [Display(Name = "Maintenance Cycle (in Days)")]
        public int? MaintenanceCycle { get; set; }

        public virtual ICollection<MaintenanceInfo> MaintenanceInfos { get; set; }
        public virtual ICollection<ShippingAssignment> ShippingAssignments { get; set; }
    }
}
