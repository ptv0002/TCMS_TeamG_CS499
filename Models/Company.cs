/*
 * Company Model 
 * Author: Veronica Vu
 * Date: 9/2/2021
 * Purpose: Provides Company model information that will be used to get Company information and set Company information in our SQL database 
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace Models
{
    [Table("Company")]
    public class Company
    {
        public Company()
        {
            OrderInfoDestinations = new HashSet<OrderInfo>();
            OrderInfoSources = new HashSet<OrderInfo>();
        }
        public int Id { get; set; }
        [DisplayName("Full Company Name")]
        public string Name { get; set; }
        public bool? Status { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int? Zip { get; set; }
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        public virtual ICollection<OrderInfo> OrderInfoDestinations { get; set; }
        public virtual ICollection<OrderInfo> OrderInfoSources { get; set; }
    }
}
