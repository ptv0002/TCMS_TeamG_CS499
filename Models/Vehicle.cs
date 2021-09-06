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
            MaintenanceInfo = new HashSet<MaintenanceInfo>();
            ShippingAssignment = new HashSet<ShippingAssignment>();
        }

        [StringLength(10)]
        public string ID { get; set; }

        [StringLength(50)]
        public string Brand { get; set; }

        public int? Year { get; set; }

        [StringLength(50)]
        public string Model { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        public bool? ReadyStatus { get; set; }

        public bool? Status { get; set; }

        public string Parts { get; set; }

        public DateTime? LastMaintenanceDate { get; set; }

        public int? MaintenanceCycle { get; set; }

        public virtual ICollection<MaintenanceInfo> MaintenanceInfo { get; set; }

        public virtual ICollection<ShippingAssignment> ShippingAssignment { get; set; }
    }
}
