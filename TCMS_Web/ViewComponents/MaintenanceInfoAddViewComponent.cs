using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMS_Web.ViewComponents
{
    public class MaintenanceInfoAddViewComponent : ViewComponent
    {
        private readonly TCMS_Context _context;
        public MaintenanceInfoAddViewComponent(TCMS_Context context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _context.MaintenanceInfos.FirstOrDefaultAsync());
        }
    }
}
