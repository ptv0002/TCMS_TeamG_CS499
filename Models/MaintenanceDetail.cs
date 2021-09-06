using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    [Table("MaintenanceDetail")]
    public class MaintenanceDetail
    {
        public int ID { get; set; }

        public int? MaintenanceInfoID { get; set; }

        [StringLength(50)]
        public string Service { get; set; }

        public double? EstimateCost { get; set; }

        public string Notes { get; set; }

        public bool? Status { get; set; }

        public virtual MaintenanceInfo MaintenanceInfo { get; set; }
    }
}
