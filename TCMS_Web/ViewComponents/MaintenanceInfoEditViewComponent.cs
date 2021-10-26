using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMS_Web.ViewComponents
{
    public class MaintenanceInfoEditViewComponent:ViewComponent
    {
        private readonly TCMS_Context _context;
        public MaintenanceInfoEditViewComponent(TCMS_Context context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? id)
        {
            return View(await _context.MaintenanceInfos.FirstOrDefaultAsync(m => m.Id == id));
        }
    }
}

