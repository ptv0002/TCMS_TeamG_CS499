using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMS_Web.Models
{
    public class GroupViewModel<T>
    {
        public StatusViewModel StatusViewModel { get; set; }
        public IEnumerable<T> ClassModel { get; set; }
    }
}
