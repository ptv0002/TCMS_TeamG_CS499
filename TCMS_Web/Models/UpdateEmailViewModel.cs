using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TCMS_Web.Models
{
    public class UpdateEmailViewModel
    {
        [Display(Name = "Employee ID")]
        public string Id { get; set; }
        [Display(Name = "Old Email")]
        public string OldEmail { get; set; }
        [Display(Name = "New Email")]
        public string NewEmail { get; set; }
    }
}
