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
            //OrderInfo = new HashSet<OrderInfo>();
           // OrderInfo1 = new HashSet<OrderInfo>();
        }

        public int ID { get; set; }

        [StringLength(50)]

        [Required(ErrorMessage ="This Field is required.")]
        [DisplayName("Full Name")]
        public string Name { get; set; }

        public bool? Status { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        public int? Zip { get; set; }

        public string ContactPerson { get; set; }

       // public virtual ICollection<OrderInfo> OrderInfo { get; set; }

        //public virtual ICollection<OrderInfo> OrderInfo1 { get; set; }
    }
}
