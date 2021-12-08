/*
 * Maintenance Model
 * Author: Veronica Vu
 * Date: 9/2/2021
 * Purpose: Provides Maintenance Detail model information that will be used to get 
 * Maintenance Detail information and set Maintenance Detail information in our SQL database 
 * 
 */

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
        public int? Id { get; set; }
        public int? MaintenanceInfoId { get; set; }
        public string Service { get; set; }
        [Display(Name = "Estimated Cost")]
        public double? EstimateCost { get; set; }
        public string Notes { get; set; }
        public bool? Status { get; set; }

        public virtual MaintenanceInfo MaintenanceInfo { get; set; }
    }
}
