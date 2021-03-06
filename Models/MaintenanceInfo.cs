/*
 * Maintenance Model
 * Author: Veronica Vu
 * Date: 9/2/2021
 * Purpose: Provides Maintenance model information that will be used to get 
 * Maintenance Detail information and set Maintenance Detail information in our SQL database 
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Table("MaintenanceInfo")]
    public class MaintenanceInfo
    {
        public MaintenanceInfo()
        {
            MaintenanceDetails = new HashSet<MaintenanceDetail>();
        }

        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string VehicleId { get; set; }
        public DateTime? Datetime { get; set; }
        public string Notes { get; set; }
        public bool? Status { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual ICollection<MaintenanceDetail> MaintenanceDetails { get; set; }
    }
}
