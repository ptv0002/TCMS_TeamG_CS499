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
            MaintenanceDetail = new HashSet<MaintenanceDetail>();
        }

        public int ID { get; set; }

        [StringLength(10)]
        public string EmployeeID { get; set; }

        [StringLength(10)]
        public string VehicleID { get; set; }

        public DateTime? Datetime { get; set; }

        public string Notes { get; set; }

        public bool? Status { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual ICollection<MaintenanceDetail> MaintenanceDetail { get; set; }

        public virtual Vehicle Vehicle { get; set; }
    }
}
