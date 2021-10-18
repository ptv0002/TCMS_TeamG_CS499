using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMS_Web.Areas.Other.Controllers
{
    [Area("Other")]
    [Route("other/[controller]")]
    public class NoRoleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
