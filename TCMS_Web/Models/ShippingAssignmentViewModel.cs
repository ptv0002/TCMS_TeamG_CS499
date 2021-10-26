using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using Models;

namespace TCMS_Web.Models
{
    public class ShippingAssignmentViewModel
    {
        public IEnumerable<Vehicle> Vehicles { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
        public IEnumerable<ShippingAssignment> ShippingAssignments { get; set; }

    }
}
