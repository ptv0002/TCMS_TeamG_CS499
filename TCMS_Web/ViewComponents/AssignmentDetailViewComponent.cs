using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;

namespace TCMS_Web.Controllers
{
    public class AssignmentDetailViewComponent : ViewComponent
    {
        private readonly TCMS_Context _context;

        public AssignmentDetailViewComponent(TCMS_Context context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //return Task.FromResult<IViewComponentResult>(View("first", _context.AssignmentDetails.ToList()));
            return View("first", _context.AssignmentDetails.ToList());
        }
    }
}
