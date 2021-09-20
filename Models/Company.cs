using System;
using System.Collections.Generic;
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
        public string Name { get; set; }
        public bool? Status { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int? Zip { get; set; }
        public string ContactPerson { get; set; }

        public virtual ICollection<OrderInfo> OrderInfoDestinations { get; set; }
        public virtual ICollection<OrderInfo> OrderInfoSources { get; set; }
    }
}
